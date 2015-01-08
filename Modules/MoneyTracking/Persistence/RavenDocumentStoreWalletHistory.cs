namespace Modules.MoneyTracking.Persistence
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
                var sources = session.Query<Source, Sources_ByName>().Where(src => src.Name == change.Source).ToList();
                if (sources.Count == 1)
                {
                    sources.First().SetBalance(change.After);
                }
                else
                {
                    session.Store(new Source(change.Source, change.After));
                }
            }
        }

        public IList<Operation> GetAll()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var query = session.Query<Operation>().OrderBy(operation => operation.When);

                if (WaitForNonStale)
                {
                    query = query.Customize(x => x.WaitForNonStaleResults());
                }

                return query.ToList();
            }
        }

        public IList<Operation> GetForMonth(int year, int month)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var date = new DateTime(year, month, 1);
                var query =
                    session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>()
                    .Where(result => result.MonthYear == date.ToString("MMyy"))
                    .OrderBy(result => result.When);

                if (WaitForNonStale)
                {
                    query = query.Customize(x => x.WaitForNonStaleResults());
                }

                return query.OfType<Operation>().ToList();
            }
        }

        public IEnumerable<Source> GetSources()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                return session.Query<Source>();
            }
        }
    }
}
