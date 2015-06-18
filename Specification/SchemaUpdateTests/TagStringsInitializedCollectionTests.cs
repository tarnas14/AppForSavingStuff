namespace Specification.SchemaUpdateTests
{
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.SchemaUpdates;
    using NUnit.Framework;

    public class TagStringsInitializedCollectionTests
    {
        private DocumentStoreProvider _storeProvider;
        private StandardBagOfRavenMagic _ravenMagic;

        [SetUp]
        public void Setup()
        {
            _storeProvider = new DocumentStoreProvider { RunInMemory = true };
            _ravenMagic = new StandardBagOfRavenMagic(_storeProvider) { WaitForNonStale = true };
        }

        [Test]
        public void ShouldMoveTagsToTagStringCollection()
        {
            //given
            using (var session = _ravenMagic.Store.OpenSession())
            {
                session.Store(new Operation
                {
                    TagStrings = null
                });
                session.SaveChanges();
            }
            var tagsUpdate = new TagStringsInitializedCollection(_ravenMagic);

            //when
            tagsUpdate.Update(() => { });

            //then
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operations = session.Query<Operation>();
                var operation = operations.Single();
                Assert.That(operation.TagStrings, Is.Not.Null);
                Assert.That(operation.TagStrings, Is.Empty);
            }
        }
    }
}