using Raven.Client.Extensions;

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
                DefaultDatabase = "Database",
                RunInMemory = RunInMemory,                
            }
            .Initialize();

            store.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists("Database");

            new Operations_ByMonthYear().Execute(store);
            new Sources_ByChangesInOperations().Execute(store);

            return store;
        }
    }
}