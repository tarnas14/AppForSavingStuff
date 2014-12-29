namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class Wallet
    {
        private readonly IDictionary<string, Moneyz> _sources;
        private readonly WalletHistory _walletHistory;
        private readonly TimeMaster _timeMaster;

        public Wallet(WalletHistory walletHistory, TimeMaster timeMaster)
        {
            _walletHistory = walletHistory;
            _timeMaster = timeMaster;
            _sources = new Dictionary<string, Moneyz>();
        }

        public void Add(string sourceName, Moneyz howMuch)
        {
            MakeSureSourceExists(sourceName);

            var before = _sources[sourceName];
            _sources[sourceName] = _sources[sourceName] + howMuch;
            var operation = new Operation
            {
                Before = before,
                After = _sources[sourceName],
                Source = sourceName,
                When = _timeMaster.Now
            };
            _walletHistory.SaveOperation(operation);
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

            var before = _sources[sourceName];
            _sources[sourceName] = _sources[sourceName] - howMuch;
            var operation = new Operation()
            {
                Before = before,
                After = _sources[sourceName],
                Source = sourceName,
                When = _timeMaster.Now
            };
            _walletHistory.SaveOperation(operation);
        }

        public void Transfer(string source, string destination, Moneyz howMuch)
        {
            MakeSureSourceExists(source);
            MakeSureSourceExists(destination);

            Subtract(source, howMuch);
            Add(destination, howMuch);
        }

        public History GetFullHistory()
        {
            return new History
            {
                Operations = _walletHistory.GetAll()
            };
        }

        public History GetHistoryForThisMonth()
        {
            var today = _timeMaster.Today;

            return new History
            {
                Operations = _walletHistory.GetForMonth(today.Year, today.Month)
            };
        }
    }
}