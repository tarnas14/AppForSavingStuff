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

        public void Add(string sourceName, OperationInput input)
        {
            MakeSureSourceExists(sourceName);

            var source = _sources[sourceName];

            var before = source.Balance;
            source.SetBalance(before + input.HowMuch);

            var operation = new Operation(_timeMaster.Now)
            {
                Description = input.Description,
                Tags = input.Tags
            };
            operation.AddChange(source.Name, before, source.Balance);

            _walletHistory.SaveOperation(operation);
        }

        public void Subtract(string sourceName, OperationInput operationInput)
        {
            MakeSureSourceExists(sourceName);

            var source = _sources[sourceName];
            var before = source.Balance;
            source.SetBalance(before - operationInput.HowMuch);

            var operation = new Operation(_timeMaster.Now)
            {
                Description = operationInput.Description,
                Tags = operationInput.Tags
            };
            operation.AddChange(source.Name, before, source.Balance);

            _walletHistory.SaveOperation(operation);
        }

        public void Transfer(string sourceName, string destinationName, OperationInput operationInput)
        {
            MakeSureSourceExists(sourceName);
            MakeSureSourceExists(destinationName);

            var operation = new Operation(_timeMaster.Now)
            {
                Description = operationInput.Description,
                Tags = operationInput.Tags
            };

            var source = _sources[sourceName];
            var before = source.Balance;
            source.SetBalance(before - operationInput.HowMuch);

            operation.AddChange(source.Name, before, source.Balance);

            var destination = _sources[destinationName];
            before = destination.Balance;
            destination.SetBalance(before + operationInput.HowMuch);

            operation.AddChange(destination.Name, before, destination.Balance);

            _walletHistory.SaveOperation(operation);
        }
    }
}