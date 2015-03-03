namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
    using System.Linq;
    using Presentation;

    public class DisplayHistoryCommandHandler : CommandHandler<DisplayHistoryCommand>
    {
        private readonly WalletUi _walletUi;
        private readonly TimeMaster _timeMaster;
        private readonly WalletHistory _walletHistory;

        public DisplayHistoryCommandHandler(WalletHistory walletHistory, WalletUi walletUi, TimeMaster timeMaster)
        {
            _walletHistory = walletHistory;
            _walletUi = walletUi;
            _timeMaster = timeMaster;
        }

        public void Execute(DisplayHistoryCommand command)
        {
            var operations = command.Monthly ? GetOperationsForThisMonth() : _walletHistory.GetFullHistory();

            if (command.Sources.Any())
            {
                operations = operations.Where(operation => OperationDealsWithAnySource(operation, command.Sources)).ToList();
            }

            _walletUi.DisplayHistory(new History()
            {
                Operations = operations
            }, command.Verbosity);
        }

        private IList<Operation> GetOperationsForThisMonth()
        {
            return _walletHistory.GetForMonth(_timeMaster.Today.Year, _timeMaster.Today.Month);
        }

        private bool OperationDealsWithAnySource(Operation operation, ICollection<string> sources)
        {
            return operation.Changes.Select(change => change.Source).Any(sources.Contains);
        }
    }
}