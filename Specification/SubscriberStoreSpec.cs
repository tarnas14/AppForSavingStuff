namespace Specification
{
    using System.Linq;
    using ConsoleUi;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class SubscriberStoreSpec
    {
        [Test]
        public void ShouldStoreSubscribersForCommands()
        {
            //given
            var subMock = new Mock<Subscriber>();
            var subStore = new SubscriberStore();

            //when
            subStore.Store(subMock.Object, "commandName");
            var subs = subStore.GetSubsFor("commandName");

            //then
            Assert.That(subs.Count(), Is.EqualTo(1));
            Assert.That(subs.First(), Is.EqualTo(subMock.Object));
        }
    }
}
