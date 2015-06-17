namespace Specification.SchemaUpdateTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.SchemaUpdates;
    using NUnit.Framework;

    class OperationChangeDifferencesTests
    {
        private BagOfRavenMagic _ravenMagic;

        [SetUp]
        public void Setup()
        {
            _ravenMagic = new StandardBagOfRavenMagic(new DocumentStoreProvider { RunInMemory = true }) {WaitForNonStale = true};
        }

        [Test]
        public void ShouldAssignDifferenceValueToEachChangInOperations()
        {
            //given
            using (var session = _ravenMagic.Store.OpenSession())
            {
                session.Store(new Operation
                {
                    Changes = new List<Change>
                    {
                        new Change
                        {
                            After = 10,
                            Before = 2,
                            Difference = null
                        }
                    }
                });
                session.SaveChanges();
            }
            var operationChangesUpdate = new OperationChangeDifferences(_ravenMagic);

            //when
            operationChangesUpdate.Update(() => { });

            //then
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operations = _ravenMagic.WaitForQueryIfNecessary(session.Query<Operation>()).Where(operation => operation.Changes.Any(change => change.Difference == null)).ToList();

                Assert.That(operations, Is.Empty);
            }
        }
    }
}
