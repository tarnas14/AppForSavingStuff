namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Linq;
    using Presentation;
    using Raven.Abstractions.Extensions;

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
            var balancesToDisplay = new Balances();
            if (ShouldDisplayAllSources(command))
            {
                LoadAllSources(balancesToDisplay);
            }
            else
            {
                command.Sources.ForEach(sourceName => balancesToDisplay.AddBalance(sourceName, _walletHistory.GetBalance(sourceName)));
            }

            _walletUi.DisplayBalance(balancesToDisplay);
        }

        private bool ShouldDisplayAllSources(DisplayBalanceCommand command)
        {
            return !command.Sources.Any();
        }

        private void LoadAllSources(Balances balancesToDisplay)
        {
            var sources = _walletHistory.GetSources();

            sources.ForEach(balancesToDisplay.AddBalance);
        }
    }
}