namespace Modules.MoneyTracking.Persistence
{
    using Raven.Client;
    using Raven.Client.Embedded;

    public class DocumentStoreProvider
    {
        public bool RunInMemory { get; set; }

        private IDocumentStore _store;

        public IDocumentStore Store
        {
            get { return _store ?? (_store = CreateStore()); }
        }

        private IDocumentStore CreateStore()
        {
            IDocumentStore store = new EmbeddableDocumentStore()
            {
                DataDirectory = "Database",
                RunInMemory = RunInMemory
            }
            .Initialize();

            new Operations_ByMonthYear().Execute(store);
            new Sources_ByChanges().Execute(store);

            return store;
        }
    }
}