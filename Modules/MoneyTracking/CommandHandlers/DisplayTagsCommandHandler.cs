namespace Modules.MoneyTracking.CommandHandlers
{
    using Presentation;

    public class DisplayTagsCommandHandler : CommandHandler<DisplayTagsCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly WalletUi _walletUi;

        public DisplayTagsCommandHandler(WalletHistory walletHistory, WalletUi walletUi)
        {
            _walletHistory = walletHistory;
            _walletUi = walletUi;
        }

        public void Handle(DisplayTagsCommand command)
        {
            _walletUi.DisplayTags(_walletHistory.GetAllTags());
        }
    }
}