namespace Modules.MoneyTracking.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public void UpdateScheme()
        {
            using (var session = Store.OpenSession())
            {
                var allOperations = GetAll<Operation>(session);
                UpdateOperations(allOperations);

                UpdateTags(allOperations, session);
            }
        }

        private IEnumerable<T> GetAll<T>(IDocumentSession session) where T : class
        {
            var all = new List<T>();
            int start = 0;
            while (true)
            {
                var current = WaitForQueryIfNecessary(session.Query<T>()).Take(1024).Skip(start).ToList();
                if (current.Count == 0)
                {
                    break;
                }

                start += current.Count;
                all.AddRange(current);
            }

            return all;
        }

        private void UpdateTags(IEnumerable<Operation> allOperations, IDocumentSession session)
        {
            var sanitizedNewTags = SchemaUpdates.SanitizeTags(allOperations.SelectMany(operation => operation.Tags));
            StoreNewTags(sanitizedNewTags, session);

            var allTags = GetAll<Tag>(session);
            var duplicatesToRemove = SchemaUpdates.FindDuplicatedTagsToRemove(allTags, () => Console.Write('.'));

            RemoveTags(duplicatesToRemove, session);
        }

        private void StoreNewTags(IEnumerable<Tag> tags, IDocumentSession session)
        {
            foreach (var tag in tags)
            {
                session.Store(tag);
            }
            session.SaveChanges();
        }

        private void RemoveTags(IEnumerable<Tag> tags, IDocumentSession session)
        {
            foreach (var tag in tags)
            {
                session.Delete(tag);
            }
            session.SaveChanges();
        }

        private void UpdateOperations(IEnumerable<Operation> allOperations)
        {
            Console.WriteLine("Adding balance differences to changes:");
            SchemaUpdates.PopulateOperationChangesWithBalanceDifferences(allOperations, () => Console.Write('.'));
            SchemaUpdates.InitializeOperationsWithEmptyTagStringsCollections(allOperations, () => Console.Write('.'));
            SchemaUpdates.MoveTagsToTagStrings(allOperations, () => Console.Write('.'));
            Console.WriteLine();
            Console.WriteLine("finished");
        }
    }
}