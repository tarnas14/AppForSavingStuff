namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class Wallet
    {
        private readonly IDictionary<string, Moneyz> _sources;

        public Wallet()
        {
            _sources = new Dictionary<string, Moneyz>();
            History = new WalletHistory();
        }

        public WalletHistory History { get; private set; }

        public void Add(string sourceName, Moneyz howMuch)
        {
            MakeSureSourceExists(sourceName);

            _sources[sourceName] = _sources[sourceName] + howMuch;
            History.SaveOperation(Operation.In(sourceName, howMuch));
        }

        private void MakeSureSourceExists(string sourceName)
        {
            if (!_sources.ContainsKey(sourceName))
            {
                _sources.Add(sourceName, new Moneyz(0));
            }
        }

        public Moneyz GetBalance(string sourceName)
        {
            return _sources[sourceName];
        }

        public void Subtract(string sourceName, Moneyz howMuch)
        {
            MakeSureSourceExists(sourceName);

            _sources[sourceName] = _sources[sourceName] - howMuch;
            History.SaveOperation(Operation.Out(sourceName, howMuch));
        }

        public void Transfer(string source, string destination, Moneyz howMuch)
        {
            MakeSureSourceExists(source);
            MakeSureSourceExists(destination);

            _sources[source] = _sources[source] - howMuch;
            _sources[destination] = _sources[destination] + howMuch;
            History.SaveOperation(Operation.Transfer(source, destination, howMuch));
        }
    }
}