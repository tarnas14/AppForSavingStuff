namespace Modules.MoneyTracking.Persistence
{
    using Raven.Client;
    using Raven.Client.Linq;

    public class StandardBagOfRavenMagic : BagOfRavenMagic
    {
        private readonly DocumentStoreProvider _documentStoreProvider;

        public StandardBagOfRavenMagic(DocumentStoreProvider documentStoreProvider)
        {
            _documentStoreProvider = documentStoreProvider;
        }

        public IDocumentStore Store
        {
            get { return _documentStoreProvider.Store; }
        }

        public bool WaitForNonStale { get; set; }

        public IRavenQueryable<TEntity> WaitForQueryIfNecessary<TEntity>(IRavenQueryable<TEntity> query) where TEntity : class
        {
            return !WaitForNonStale ? query : query.Customize(q => q.WaitForNonStaleResultsAsOfNow());
        }
    }
}