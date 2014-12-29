namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class Wallet
    {
        private readonly IDictionary<string, Moneyz> _sources;
        private readonly WalletHistory _walletHistory;

        public Wallet(WalletHistory walletHistory)
        {
            _walletHistory = walletHistory;
            _sources = new Dictionary<string, Moneyz>();
        }

        public void Add(string sourceName, Moneyz howMuch)
        {
            MakeSureSourceExists(sourceName);

            _sources[sourceName] = _sources[sourceName] + howMuch;
            _walletHistory.SaveOperation(Operation.In(sourceName, howMuch));
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
            _walletHistory.SaveOperation(Operation.Out(sourceName, howMuch));
        }

        public void Transfer(string source, string destination, Moneyz howMuch)
        {
            MakeSureSourceExists(source);
            MakeSureSourceExists(destination);

            _sources[source] = _sources[source] - howMuch;
            _sources[destination] = _sources[destination] + howMuch;
            _walletHistory.SaveOperation(Operation.Transfer(source, destination, howMuch));
        }

        public History GetFullHistory()
        {
            return new History
            {
                Operations = _walletHistory.GetAll()
            };
        }
    }
}