namespace Modules.MoneyTracking.CommandHandlers
{
    public class DisplayBalanceCommandHandler : CommandHandler<DisplayBalanceCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly WalletUi _walletUi;

        public DisplayBalanceCommandHandler(WalletHistory walletHistory, WalletUi walletUi)
        {
            _walletHistory = walletHistory;
            _walletUi = walletUi;
        }

        public void Execute(DisplayBalanceCommand command)
        {
            var balance = _walletHistory.GetBalance(command.SourceName);

            _walletUi.DisplayBalance(command.SourceName, balance);
        }
    }
}