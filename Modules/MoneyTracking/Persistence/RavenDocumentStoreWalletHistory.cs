namespace Modules.MoneyTracking.Persistence
{
    using System;
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

        public void SaveOperation(Operation toSave)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                session.Store(toSave);
                session.SaveChanges();
            }
        }

        public IList<Operation> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<Operation> GetForMonth(int year, int month)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var date = new DateTime(year, month, 1);
                var query =
                    session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>()
                    .Where(operation => operation.MonthYear == date.ToString("MMyy"));

                if (WaitForNonStale)
                {
                    query = query.Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(2)));
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
