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
                var operationsWithoutDifference = GetOperationsWithoutDifference(session);
                while (operationsWithoutDifference.Any())
                {
                    foreach (var change in operationsWithoutDifference.SelectMany(operations => operations.Changes))
                    {
                        change.Difference = change.After - change.Before;
                        progressAction();
                    }
                }
                session.SaveChanges();
            }
        }

        private IEnumerable<Operation> GetOperationsWithoutDifference(IDocumentSession session)
        {
            return _ravenMagic.WaitForQueryIfNecessary(session.Query<Operation>()).Where(operation => operation.Changes.Any(change => change.Difference == null));
        }
    }
}