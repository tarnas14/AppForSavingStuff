namespace Modules.MoneyTracking.Persistence
{
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Client.Linq;

    public class RavenDocumentStoreWalletHistory : WalletHistory
    {
        private readonly DocumentStoreProvider _storeProvider;

        public RavenDocumentStoreWalletHistory(DocumentStoreProvider storeProvider)
        {
            _storeProvider = storeProvider;
        }

        public bool WaitForNonStale { get; set; }

        public IList<Operation> GetFullHistory()
        {
            var operations = WaitForQueryIfNecessary(QueryOperations()).OrderBy(operation => operation.When).OfType<Operation>().ToList();

            LegacyDataMagic.AddDifferencesToChanges(operations.SelectMany(operation => operation.Changes));

            return operations.ToList();
        }

        public IRavenQueryable<Operations_ByMonthYear.Result> QueryOperations()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                return WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>());
            }
        }

        private IRavenQueryable<Operations_ByMonthYear.Result> ByMonth(IRavenQueryable<Operations_ByMonthYear.Result> query, Month month)
        {
            return WaitForQueryIfNecessary(query.Where(result => result.MonthYear == month.GetIndexString()));
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

        public IList<Tag> GetAllTags()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var tags = WaitForQueryIfNecessary(session.Query<Tag>());

                return tags.ToList();
            }
        }

        public void RemoveSource(string source)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var operations =
                    WaitForQueryIfNecessary(session.Query<Operations_BySources.Result, Operations_BySources>())
                        .Where(operation => operation.SourceName == source)
                        .OfType<Operation>()
                        .ToList();

                operations.ForEach(session.Delete);
                session.SaveChanges();
            }
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
                    sources =
                        WaitForQueryIfNecessary(session.Query<Source, Sources_ByChangesInOperations>())
                        .Where(src => src.Name == sourceName)
                        .ToList();
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

            var tagOperations = history.Where(operation => operation.TagStrings.Any(tagString => tagString == tagName));

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
