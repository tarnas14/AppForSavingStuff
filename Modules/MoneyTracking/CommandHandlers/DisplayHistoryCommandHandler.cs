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
            var operations = GetOperations(command.Monthly, command.Month);

            if (command.Sources.Any())
            {
                operations = operations.Where(operation => OperationDealsWithAnySource(operation, command.Sources)).ToList();
            }

            _walletUi.DisplayHistory(new History()
            {
                Operations = operations
            }, new HistoryDisplayVerbosity
            {
                Descriptions = command.DisplayDescriptions,
                Tags = command.DisplayTags
            });
        }

        private IList<Operation> GetOperations(bool monthly, Month month)
        {
            if (month != null)
            {
                return _walletHistory.GetForMonth(month);
            }

            if (monthly)
            {
                return _walletHistory.GetForMonth(new Month(_timeMaster.Today.Year, _timeMaster.Today.Month));
            }

            return _walletHistory.GetFullHistory();
        }

        private bool OperationDealsWithAnySource(Operation operation, ICollection<string> sources)
        {
            var tagSources = sources.Where(Tag.IsTagName).Select(Tag.GetSanitizedTagName);
            var dealsWithTagSource = (tagSources.Any() && operation.Tags.Any(tag => tagSources.Contains(tag.Value)));

            var regularSources = sources.Where(source => !Tag.IsTagName(source));
            var dealsWithRegularSource = (regularSources.Any() && operation.Changes.Select(change => change.Source).Any(regularSources.Contains));

            return dealsWithTagSource || dealsWithRegularSource;
        }
    }
}