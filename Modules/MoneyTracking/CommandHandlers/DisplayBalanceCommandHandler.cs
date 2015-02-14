namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
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
            if (command.SourceName == "all")
            {
                LoadAllSources(balancesToDisplay);
            }
            else
            {
                var balance = _walletHistory.GetBalance(command.SourceName);
                balancesToDisplay.AddBalance(command.SourceName, balance);
            }

            _walletUi.DisplayBalance(balancesToDisplay);
        }

        private void LoadAllSources(Balances balancesToDisplay)
        {
            var sources = _walletHistory.GetSources();

            sources.ForEach(balancesToDisplay.AddBalance);
        }
    }
}