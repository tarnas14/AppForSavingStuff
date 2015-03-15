﻿namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
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
                CheckIfAllSourcesAreOfTheSameType(command.Sources);
                command.Sources.ForEach(sourceName => balancesToDisplay.AddBalance(sourceName, GetBalance(sourceName, command.Month)));
            }

            balancesToDisplay.DisplaySum = !OnlyTagsInSources(command.Sources);

            _walletUi.DisplayBalance(balancesToDisplay);
        }

        private void CheckIfAllSourcesAreOfTheSameType(IEnumerable<string> sources)
        {
            if (!OnlyTagsInSources(sources))
            {
                throw new WalletException("Cannot mix sources and tags in balance command.");
            }
        }

        private static bool OnlyTagsInSources(IEnumerable<string> sources)
        {
            return sources.Any(Tag.IsTagName) && sources.All(Tag.IsTagName);
        }

        private Moneyz GetBalance(string sourceName, Month month)
        {
            if (month == null)
            {
                return _walletHistory.GetBalance(sourceName);
            }

            return _walletHistory.GetSourceBalanceForMonth(sourceName, month);
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