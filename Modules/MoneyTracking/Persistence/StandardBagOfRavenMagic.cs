namespace Modules.MoneyTracking.Persistence
{
    using System;
    using Raven.Client;
    using Raven.Client.Linq;
    using SchemaUpdates;

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

        public void UpdateScheme()
        {
            UpdateTags();
            UpdateOperations();
        }

        private void UpdateTags()
        {
            Console.WriteLine("Updating tags:");
            var tags = new MoveTagsToTagStringsAndStoreTags(this);
            tags.Update(() => Console.Write('.'));
            Console.WriteLine("tags updated");
        }

        private void UpdateOperations()
        {
            Console.WriteLine("Updating operations:");
            var operationUpdate = new OperationChangeDifferences(this);
            operationUpdate.Update(() => Console.Write('.'));
            Console.WriteLine("operations updated");
        }
    }
}