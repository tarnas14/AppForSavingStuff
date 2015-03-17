namespace Modules.MoneyTracking.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Abstractions.Extensions;
    using Raven.Client;
    using Raven.Client.Linq;

    public class RavenDocumentStoreWalletHistory : WalletHistory
    {
        private readonly DocumentStoreProvider _storeProvider;

        public RavenDocumentStoreWalletHistory(DocumentStoreProvider storeProvider)
        {
            _storeProvider = storeProvider;
        }

        public bool WaitForNonStale { get; set; }

        public void SaveOperation(Operation toSave)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                AdditionalInformationToChanges(toSave.Changes);
                session.Store(toSave);
                session.SaveChanges();
            }
        }

        private void AdditionalInformationToChanges(IEnumerable<Change> changes)
        {
            foreach (var change in changes)
            {
                var source = GetSourceByName(change.Source);
                if (source != null)
                {
                    change.Before = source.Balance;
                    change.After = source.Balance + change.Difference;
                }
                else
                {
                    change.Before = new Moneyz(0);
                    change.After = change.Difference;
                }
            }
        }

        public IList<Operation> GetFullHistory()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var operations = WaitForQueryIfNecessary(session.Query<Operation>().OrderBy(operation => operation.When)).ToList();

                LegacyDataMagic.AddDifferencesToChanges(operations.SelectMany(operation => operation.Changes));

                return operations.ToList();
            }
        }

        public IList<Operation> GetForMonth(Month month)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var date = new DateTime(month.Year, month.MonthNr, 1);
                return GetMonthHistory(session, date).ToList();
            }
        }

        private IEnumerable<Operation> GetMonthHistory(IDocumentSession session, DateTime date)
        {
            var operations = WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>())
                .Where(result => result.MonthYear == date.ToString("MMyy"))
                .OrderBy(result => result.When).OfType<Operation>().ToList();

            LegacyDataMagic.AddDifferencesToChanges(operations.SelectMany(operation => operation.Changes));

            return operations;
        }

        public IList<Source> GetSources()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var results = WaitForQueryIfNecessary(session.Query<Sources_ByChangesInOperations.Result, Sources_ByChangesInOperations>()).ToList();
                return Source.FromMapReduceResults(results);
            }
        }

        public Moneyz GetBalance(string sourceName)
    {
            var source = GetSourceByName(sourceName);

            if (source == null)
            {
                throw new SourceDoesNotExistException(sourceName);
            }

            return source.Balance;
        }

        public Moneyz GetSourceBalanceForMonth(string sourceName, Month month)
        {
            var date = new DateTime(month.Year, month.MonthNr, 1);

            if (Tag.IsTagName(sourceName))
            {
                var tagOperationsThisMonth = GetTagHistoryForThisMonth(Tag.GetSanitizedTagName(sourceName), month);
                var changesForTagThisMonth = tagOperationsThisMonth.SelectMany(operation => operation.Changes);

                var tagMoneyBalanceForMonth = changesForTagThisMonth.Sum(change => change.Difference);

                return tagMoneyBalanceForMonth; 
            }

            using (var session = _storeProvider.Store.OpenSession())
            {
                var monthHistory = GetMonthHistory(session, date);

                var changesOnSourceThisMonth =
                    monthHistory.SelectMany(operation => operation.Changes.Where(change => change.Source == sourceName));

                var stateBeforeThisMonth = changesOnSourceThisMonth.First().Before;
                var lastChangeInThisMonth = changesOnSourceThisMonth.Last().After;

                return lastChangeInThisMonth - stateBeforeThisMonth;
            }
        }

        public IList<Operation> GetTagHistoryForThisMonth(string tagName, Month month)
        {
            var date = new DateTime(month.Year, month.MonthNr, 1);

            using (var session = _storeProvider.Store.OpenSession())
            {
                var operations = WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>())
                    .Where(result => result.MonthYear == date.ToString("MMyy") && result.TagNames.Any(tag => tag == tagName))
                    .OrderBy(result => result.When).OfType<Operation>().ToList();

                LegacyDataMagic.AddDifferencesToChanges(operations.SelectMany(operation => operation.Changes));

                return operations;
            }
        }

        public IList<Tag> GetTagsForMonth(Month month)
        {
            var thisMonthOperations = GetForMonth(month);

            var tagsInOperations = thisMonthOperations.SelectMany(operation => operation.Tags).Distinct();

            return tagsInOperations.ToList();
        }

        public IList<Tag> GetAllTags()
        {
            var historyOperations = GetFullHistory();

            var tags = historyOperations.SelectMany(operation => operation.Tags).Distinct();

            return tags.ToList();
        }

        private Source GetSourceByName(string sourceName)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                IList<Source> sources = new List<Source>();

                if (Tag.IsTagName(sourceName))
                {
                    sources.Add(GetSourceFromTag(sourceName));
                }
                else
                {
                    var results =
                        WaitForQueryIfNecessary(session.Query<Sources_ByChangesInOperations.Result, Sources_ByChangesInOperations>())
                        .Where(src => src.Name == sourceName)
                        .ToList();
                    sources = Source.FromMapReduceResults(results);
                }

                if (sources.Count == 1)
                {
                    return sources.First();
                }

                return null;
            }
        }

        private Source GetSourceFromTag(string tagName)
        {
            var history = GetFullHistory();

            var tagNameWithoutHash = tagName.Substring(1);
            var tagOperations = history.Where(operation => operation.Tags.Any(tag => tag.Value == tagNameWithoutHash));

            var changes = tagOperations.SelectMany(operation => operation.Changes);

            return new Source
            {
                Name = tagName,
                Balance = changes.Aggregate(new Moneyz(0), (money, change) => money + change.Difference)
            };
        }

        private IRavenQueryable<TEntity> WaitForQueryIfNecessary<TEntity>(IRavenQueryable<TEntity> query) where TEntity : class
        {
            return !WaitForNonStale ? query : query.Customize(q => q.WaitForNonStaleResultsAsOfNow());
        }
    }
}
