namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class Wallet
    {
        private readonly IDictionary<string, Source> _sources; 
        private WalletHistory _walletHistory;
        private readonly TimeMaster _timeMaster;

        public Wallet(WalletHistory walletHistory, TimeMaster timeMaster)
        {
            _timeMaster = timeMaster;
            _sources = new Dictionary<string, Source>();
            LoadHistory(walletHistory);
        }

        private void LoadHistory(WalletHistory walletHistory)
        {
            var sources = walletHistory.GetSources();

            foreach (var source in sources)
            {
                _sources.Add(source.Name, source);
            }

            _walletHistory = walletHistory;
        }

        public void Add(string sourceName, Moneyz howMuch)
        {
            MakeSureSourceExists(sourceName);

            var source = _sources[sourceName];

            var before = source.Balance;
            source.SetBalance(before + howMuch);

            var operation = new Operation(_timeMaster.Now);
            operation.AddChange(source.Name, before, source.Balance);

            _walletHistory.SaveOperation(operation);
        }

        private void MakeSureSourceExists(string sourceName)
        {
            if (!_sources.ContainsKey(sourceName))
            {
                _sources.Add(sourceName, new Source(sourceName));
            }
        }

        public Moneyz GetBalance(string sourceName)
        {
            return _sources[sourceName].Balance;
        }

        public void Subtract(string sourceName, Moneyz howMuch)
        {
            MakeSureSourceExists(sourceName);

            var source = _sources[sourceName];
            var before = source.Balance;
            source.SetBalance(before - howMuch);

            var operation = new Operation(_timeMaster.Now);
            operation.AddChange(source.Name, before, source.Balance);

            _walletHistory.SaveOperation(operation);
        }

        public void Transfer(string sourceName, string destinationName, Moneyz howMuch)
        {
            MakeSureSourceExists(sourceName);
            MakeSureSourceExists(destinationName);

            var operation = new Operation(_timeMaster.Now);

            var source = _sources[sourceName];
            var before = source.Balance;
            source.SetBalance(before - howMuch);

            operation.AddChange(source.Name, before, source.Balance);

            var destination = _sources[destinationName];
            before = destination.Balance;
            destination.SetBalance(before + howMuch);

            operation.AddChange(destination.Name, before, destination.Balance);

            _walletHistory.SaveOperation(operation);
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