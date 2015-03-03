﻿namespace Modules.MoneyTracking.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                session.Store(toSave);
                SaveSources(toSave.Changes, session);
                session.SaveChanges();
            }
        }

        private void SaveSources(IEnumerable<Change> changes, IDocumentSession session)
        {
            foreach (var change in changes)
            {
                var source = GetSourceByName(change.Source);
                if (source != null)
                {
                    session.Load<Source>(source.Id).SetBalance(change.After);
                }
                else
                {
                    session.Store(new Source(change.Source, change.After));
                }
            }
        }

        public IList<Operation> GetFullHistory()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                return WaitForQueryIfNecessary(session.Query<Operation>().OrderBy(operation => operation.When)).ToList();
            }
        }

        public IList<Operation> GetForMonth(int year, int month)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var date = new DateTime(year, month, 1);
                return GetMonthHistory(session, date).ToList();
            }
        }

        private IEnumerable<Operation> GetMonthHistory(IDocumentSession session, DateTime date)
        {
            return WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>())
                .Where(result => result.MonthYear == date.ToString("MMyy"))
                .OrderBy(result => result.When).OfType<Operation>().ToList();
        }

        public IList<Source> GetSources()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                return session.Query<Source>().ToList();
            }
        }

        public Moneyz GetBalance(string sourceName)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var source = GetSourceByName(sourceName);

                if (source == null)
                {
                    throw new SourceDoesNotExistException(sourceName);
                }

                return source.Balance;
            }
        }

        public Moneyz GetSourceBalanceForThisMonth(string sourceName, int year, int month)
        {
            var date = new DateTime(year, month, 1);

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

        public void CreateSource(string sourceName)
        {
            if (Exists(sourceName))
            {
                throw new SourceAlreadyExistsException(sourceName);
            }

            using (var session = _storeProvider.Store.OpenSession())
            {
                session.Store(new Source(sourceName));
                session.SaveChanges();
            }
        }

        public IList<Operation> GetTagHistoryForThisMonth(string tagName, int year, int month)
        {
            var date = new DateTime(year, month, 1);

            using (var session = _storeProvider.Store.OpenSession())
            {
                return WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>())
                    .Where(result => result.MonthYear == date.ToString("MMyy") && result.TagNames.Any(tag => tag == tagName))
                    .OrderBy(result => result.When).OfType<Operation>().ToList();
            }
        }

        public IList<Tag> GetTagsForMonth(int year, int month)
        {
            var thisMonthOperations = GetForMonth(year, month);

            var tagsInOperations = thisMonthOperations.SelectMany(operation => operation.Tags).Distinct();

            return tagsInOperations.ToList();
        }

        public IList<Tag> GetAllTags()
        {
            var historyOperations = GetFullHistory();

            var tags = historyOperations.SelectMany(operation => operation.Tags).Distinct();

            return tags.ToList();
        }

        public bool Exists(string sourceName)
        {
            return null != GetSourceByName(sourceName);
        }

        private Source GetSourceByName(string sourceName)
        {
            using (var session = _storeProvider.Store.OpenSession())
            { 
                var sources = WaitForQueryIfNecessary(session.Query<Source, Sources_ByName>()).Where(src => src.Name == sourceName).ToList();

                if (sources.Count == 1)
                {
                    return sources.First();
                }

                return null;
            }
        }

        private IRavenQueryable<TEntity> WaitForQueryIfNecessary<TEntity>(IRavenQueryable<TEntity> query) where TEntity : class
        {
            return !WaitForNonStale ? query : query.Customize(q => q.WaitForNonStaleResultsAsOfNow());
        }
    }
}
