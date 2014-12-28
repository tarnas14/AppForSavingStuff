namespace Specification
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
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

    internal class SubscriberStore
    {
        private readonly IDictionary<string, ICollection<Subscriber>> _subDictionary;

        public SubscriberStore()
        {
            _subDictionary = new Dictionary<string, ICollection<Subscriber>>();
        }

        public void Store(Subscriber subscriber, string commandName)
        {
            if (!_subDictionary.ContainsKey(commandName))
            {
                _subDictionary.Add(commandName, new Collection<Subscriber>{subscriber});
                return;
            }

            _subDictionary[commandName].Add(subscriber);
        }

        public IEnumerable<Subscriber> GetSubsFor(string commandName)
        {
            if (!_subDictionary.ContainsKey(commandName))
            {
                return new Collection<Subscriber>();
            }

            return _subDictionary[commandName];
        }
    }
}
