namespace Modules.MoneyTracking
{
    public class Wallet
    {
        private readonly WalletHistory _walletHistory;
        private readonly TimeMaster _timeMaster;

        public Wallet(WalletHistory walletHistory, TimeMaster timeMaster)
        {
            _walletHistory = walletHistory;
            _timeMaster = timeMaster;
        }

        public Moneyz GetBalance(string sourceName)
        {
            return _walletHistory.GetBalance(sourceName);
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
            var before = _walletHistory.GetBalance(sourceName);
            var operation = new Operation(_timeMaster.Now)
            {
                Description = input.Description,
                Tags = input.Tags
            };
            operation.AddChange(sourceName, before, before + input.HowMuch);

            _walletHistory.SaveOperation(operation);
        }

        public void Subtract(string sourceName, OperationInput operationInput)
        {
            var before = _walletHistory.GetBalance(sourceName);

            var operation = new Operation(_timeMaster.Now)
            {
                Description = operationInput.Description,
                Tags = operationInput.Tags
            };
            operation.AddChange(sourceName, before, before - operationInput.HowMuch);

            _walletHistory.SaveOperation(operation);
        }

        public void Transfer(string sourceName, string destinationName, OperationInput operationInput)
        {
            var operation = new Operation(_timeMaster.Now)
            {
                Description = operationInput.Description,
                Tags = operationInput.Tags
            };

            var before = _walletHistory.GetBalance(sourceName);
            operation.AddChange(sourceName, before, before - operationInput.HowMuch);

            before = _walletHistory.GetBalance(destinationName);
            operation.AddChange(destinationName, before, before + operationInput.HowMuch);

            _walletHistory.SaveOperation(operation);
        }
    }
}