namespace Specification.SchemaUpdateTests
{
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.SchemaUpdates;
    using NUnit.Framework;

    class TagsUpdateTests
    {
        private DocumentStoreProvider _storeProvider;
        private BagOfRavenMagic _ravenMagic;

        [SetUp]
        public void Setup()
        {
            _storeProvider = new DocumentStoreProvider{RunInMemory = true};
            _ravenMagic = new StandardBagOfRavenMagic(_storeProvider){WaitForNonStale = true};
        }

        [Test]
        public void ShouldMoveTagsToTagStringCollection()
        {
            //given
            using (var session = _ravenMagic.Store.OpenSession())
            {
                session.Store(new Operation
                {
                    Tags = new[] {new Tag("#asdf"), new Tag("#qwerty")}
                });
                session.SaveChanges();
            }
            var tagsUpdate = new MoveTagsToTagStringsAndStoreTags(_ravenMagic);

            //when
            tagsUpdate.Update(() => { });

            //then
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operations = session.Query<Operation>();
                var operation = operations.Single();
                Assert.That(operation.TagStrings, Is.EquivalentTo(new[]{"#asdf", "#qwerty"}));
                Assert.That(operation.Tags, Is.Null);
            }
        }

        [Test]
        public void ShouldPrependHashSignsToTagsThatDontStartWithOne()
        {
            //given
            using (var session = _ravenMagic.Store.OpenSession())
            {
                session.Store(new Operation
                {
                    Tags = new[] { new Tag("asdf"), new Tag("#qwerty") }
                });
                session.SaveChanges();
            }
            var tagsUpdate = new MoveTagsToTagStringsAndStoreTags(_ravenMagic);

            //when
            tagsUpdate.Update(() => { });

            //then
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operations = session.Query<Operation>();
                var operation = operations.Single();
                Assert.That(operation.TagStrings, Is.EquivalentTo(new[] { "#asdf", "#qwerty" }));
                Assert.That(operation.Tags, Is.Null);
                var tags = session.Query<Tag>().ToList();
                Assert.That(tags.All(tag => Tag.IsTagName(tag.Value)));
            }
        }
    }
}
