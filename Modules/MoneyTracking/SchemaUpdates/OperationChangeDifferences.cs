namespace Modules.MoneyTracking.SchemaUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Persistence;
    using Raven.Client;

    public class OperationChangeDifferences
    {
        private readonly BagOfRavenMagic _ravenMagic;

        public OperationChangeDifferences(BagOfRavenMagic ravenMagic)
        {
            _ravenMagic = ravenMagic;
        }

        public void Update(Action progressAction)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operationsWithoutDifference = GetAllOperationsWithoutDifference(session);
                foreach (var change in operationsWithoutDifference.SelectMany(operations => operations.Changes))
                {
                    change.Difference = change.After - change.Before;
                    progressAction();
                }
                session.SaveChanges();
            }
        }

        private IEnumerable<Operation> GetAllOperationsWithoutDifference(IDocumentSession session)
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

            return allOperations.Where(operation => operation.Changes.Any(change => change.Difference == null));
        }
    }
}