namespace Specification.SchemaUpdateTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;

    class OperationChangeDifferencesTests
    {
        [Test]
        public void ShouldAssignDifferenceValueToEachChangeInOperations()
        {
            //given
            var operations = new[]
            {
                new Operation
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
                }
            };

            //when
            SchemaUpdates.PopulateOperationChangesWithBalanceDifferences(operations, () => { });

            //then
            Assert.That(operations.Where(operation => operation.Changes.Any(change => change.Difference == null)), Is.Empty);
        }
    }
}
