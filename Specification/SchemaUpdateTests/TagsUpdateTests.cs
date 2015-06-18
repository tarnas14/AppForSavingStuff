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
        private MoveTagsToTagStringsAndStoreTags _tagsUpdate;

        [SetUp]
        public void Setup()
        {
            _storeProvider = new DocumentStoreProvider{RunInMemory = true};
            _ravenMagic = new StandardBagOfRavenMagic(_storeProvider){WaitForNonStale = true};
            _tagsUpdate = new MoveTagsToTagStringsAndStoreTags(_ravenMagic);
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

            //when
            _tagsUpdate.Update(() => { });

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

            //when
            _tagsUpdate.Update(() => { });

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

        [Test]
        public void ShouldGetRidOfDuplicates()
        {
            //given
            const string tagString = "#tagString";

            using (var session = _ravenMagic.Store.OpenSession())
            {
                session.Store(new Tag(tagString));
                session.Store(new Tag(tagString));
                session.Store(new Operation
                {
                    Tags = new[] { new Tag(tagString) }
                });
                session.Store(new Operation
                {
                    Tags = new[] { new Tag(tagString) }
                });
                session.SaveChanges();
            }

            //when
            _tagsUpdate.Update(() => {});

            //then
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var tags = _ravenMagic.WaitForQueryIfNecessary(session.Query<Tag>()).ToList();
                Assert.That(tags.Single().Value, Is.EqualTo(tagString));
            }
        }
    }
}
