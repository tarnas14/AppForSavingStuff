namespace Specification.SchemaUpdateTests
{
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;

    public class TagStringsInitializedCollectionTests
    {
        [Test]
        public void ShouldMoveTagsToTagStringCollection()
        {
            //given
            var operations = new[] {
                new Operation
                {
                    TagStrings = null
                }
            };

            //when
            SchemaUpdates.InitializeOperationsWithEmptyTagStringsCollections(operations, () => { });

            //then
            var operation = operations.Single();
            Assert.That(operation.TagStrings, Is.Not.Null);
            Assert.That(operation.TagStrings, Is.Empty);
        }
    }
}