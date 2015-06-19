namespace Specification.SchemaUpdateTests
{
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;

    class TagsUpdateTests
    {
        [Test]
        public void ShouldMoveTagsToTagStringCollection()
        {
            //given
            var operations = new [] {
                new Operation
                {
                    Tags = new[] {new Tag("#asdf"), new Tag("#qwerty")}
                }
            };

            //when
            SchemaUpdates.MoveTagsToTagStrings(operations, () => {});

            //then
            var operation = operations.Single();
            Assert.That(operation.TagStrings, Is.EquivalentTo(new[]{"#asdf", "#qwerty"}));
            Assert.That(operation.Tags, Is.Null);
        }

        [Test]
        public void ShouldPrependHashSignsToTagsThatDontStartWithOne()
        {
            //given
            var operations = new[]
            {
                new Operation
                {
                    Tags = new[] {new Tag("asdf"), new Tag("#qwerty")}
                }
            };

            //when
            SchemaUpdates.MoveTagsToTagStrings(operations, () => { });

            //then
            var operation = operations.Single();
            Assert.That(operation.TagStrings, Is.EquivalentTo(new[] { "#asdf", "#qwerty" }));
            Assert.That(operation.Tags, Is.Null);
        }

        [Test]
        public void ShouldGetRidOfDuplicates()
        {
            //given
            const string tagString = "#tagString";
            const string anotherTagString = "#asdf";

            var tags = new[]
            {
                new Tag(tagString),
                new Tag(tagString),
                new Tag(tagString),
                new Tag(anotherTagString),
                new Tag(anotherTagString),
            };

            //when
            var duplicatesToRemove = SchemaUpdates.FindDuplicatedTagsToRemove(tags, () => { });

            //then
            Assert.That(duplicatesToRemove.Count(), Is.EqualTo(3));
            Assert.That(duplicatesToRemove.Count(tag => tag.Value == tagString), Is.EqualTo(2));
            Assert.That(duplicatesToRemove.Count(tag => tag.Value == anotherTagString), Is.EqualTo(1));
        }
    }
}
