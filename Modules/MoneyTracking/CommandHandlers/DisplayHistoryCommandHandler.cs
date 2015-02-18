namespace Modules.MoneyTracking.CommandHandlers
{
    using Presentation;

    public class DisplayHistoryCommandHandler : CommandHandler<DisplayHistoryCommand>
    {
        private readonly WalletUi _walletUi;
        private readonly WalletHistory _walletHistory;

        public DisplayHistoryCommandHandler(WalletHistory walletHistory, WalletUi walletUi)
        {
            _walletHistory = walletHistory;
            _walletUi = walletUi;
        }

        public void Execute(DisplayHistoryCommand command)
        {
            var allOperations = _walletHistory.GetFullHistory();

            _walletUi.DisplayHistory(new History()
            {
                Operations = allOperations
            }, new HistoryDisplayVerbosity());
        }
    }
}