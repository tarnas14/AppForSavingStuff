namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class Wallet
    {
        private readonly IDictionary<string, Moneyz> _sources;
        private readonly WalletHistory _walletHistory;
        private readonly OperationFactory _operationFactory;
        private readonly TimeMaster _timeMaster;

        public Wallet(WalletHistory walletHistory, OperationFactory operationFactory, TimeMaster timeMaster)
        {
            _walletHistory = walletHistory;
            _operationFactory = operationFactory;
            _timeMaster = timeMaster;
            _sources = new Dictionary<string, Moneyz>();
        }

        public void Add(string sourceName, Moneyz howMuch)
        {
            MakeSureSourceExists(sourceName);

            _sources[sourceName] = _sources[sourceName] + howMuch;
            _walletHistory.SaveOperation(_operationFactory.GetInOperation(sourceName, howMuch));
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
            _walletHistory.SaveOperation(_operationFactory.GetOutOperation(sourceName, howMuch));
        }

        public void Transfer(string source, string destination, Moneyz howMuch)
        {
            MakeSureSourceExists(source);
            MakeSureSourceExists(destination);

            _sources[source] = _sources[source] - howMuch;
            _sources[destination] = _sources[destination] + howMuch;
            _walletHistory.SaveOperation(_operationFactory.GetTransferOperation(source, destination, howMuch));
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