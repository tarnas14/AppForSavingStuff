namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Abstractions.Extensions;

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

        public Moneyz DisplayMonthBalance(string sourceName)
        {
            var today = _timeMaster.Today;

            return _walletHistory.GetSourceBalanceForThisMonth(sourceName, today.Year, today.Month);
        }

        public void CreateSource(string sourceName)
        {
            ValidateSourceName(sourceName);

            _walletHistory.CreateSource(sourceName);
        }

        private void ValidateSourceName(string sourceName)
        {
            if (ReservedWords.Contains(sourceName))
            {
                var errorMessage = string.Format("'{0}' is a reserved word and cannot be used as a source name.", sourceName);

                throw new WalletException(errorMessage);
            }
        }

        public IList<string> ReservedWords
        {
            get { 
                return new List<string>
                {
                    "tags",
                    "all"
                };
            }
        }

        public TagHistory GetTagHistoryForThisMonth(string tagName)
        {
            var today = _timeMaster.Today;

            var operationsHistory =
                _walletHistory.GetTagHistoryForThisMonth(tagName, today.Year, today.Month);
            var changesBySource = operationsHistory.SelectMany(operation => operation.Changes)
                    .GroupBy(change => change.Source);

            var history = new Dictionary<string, Moneyz>();

            foreach (var changeGroup in changesBySource)
            {
                var combinedValue = changeGroup.Select(change => change.After - change.Before)
                    .Aggregate(new Moneyz(0), (m1, m2) => m1 + m2);

                history.Add(changeGroup.Key, combinedValue);
            }

            return new TagHistory
            {
                Tag = new Tag(tagName),
                Operations = history
            };
        }
    }
}