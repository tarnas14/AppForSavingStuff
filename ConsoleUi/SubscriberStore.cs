namespace ConsoleUi
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class SubscriberStore
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