namespace Modules.MoneyTracking
{
    using CommandHandlers;
    using Presentation;

    public class RemoveSourceCommandHandler : CommandHandler<RemoveSourceCommand>
    {
        private readonly WalletHistory _ravenHistory;
        private readonly WalletUi _walletUi;

        public RemoveSourceCommandHandler(WalletHistory ravenHistory, WalletUi walletUi)
        {
            _ravenHistory = ravenHistory;
            _walletUi = walletUi;
        }

        public void Handle(RemoveSourceCommand command)
        {
            _ravenHistory.RemoveSource(command.Source);

            _walletUi.DisplayInformation(string.Format("{0} removed", command.Source));
        }
    }
}