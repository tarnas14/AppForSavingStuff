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

        private IRavenQueryable<TEntity> WaitForQueryIfNecessary<TEntity>(IRavenQueryable<TEntity> query) where TEntity : class
        {
            return !WaitForNonStale ? query : query.Customize(q => q.WaitForNonStaleResultsAsOfNow());
        }
    }
}
