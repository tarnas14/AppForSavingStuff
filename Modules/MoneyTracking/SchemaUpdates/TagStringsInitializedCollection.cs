namespace Modules.MoneyTracking.SchemaUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Persistence;
    using Raven.Client;

    public class TagStringsInitializedCollection
    {
        private readonly StandardBagOfRavenMagic _ravenMagic;

        public TagStringsInitializedCollection(StandardBagOfRavenMagic ravenMagic)
        {
            _ravenMagic = ravenMagic;
        }

        public void Update(Action progressAction)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operationsWithNullTagStrings = GetAllOperationsWithNullTagStrings(session);

                foreach (var operationWithNullTagStrings in operationsWithNullTagStrings)
                {
                    operationWithNullTagStrings.TagStrings = new List<string>();
                    progressAction();
                }
                session.SaveChanges();
            }
        }

        private IEnumerable<Operation> GetAllOperationsWithNullTagStrings(IDocumentSession session)
        {
            var allOperations = new List<Operation>();
            int start = 0;
            while (true)
            {
                var current = _ravenMagic.WaitForQueryIfNecessary(session.Query<Operation>()).Take(1024).Skip(start).ToList();
                if (current.Count == 0)
                {
                    break;
                }

                start += current.Count;
                allOperations.AddRange(current);
            }

            return allOperations.Where(operation => operation.TagStrings == null);
        }
    }
}