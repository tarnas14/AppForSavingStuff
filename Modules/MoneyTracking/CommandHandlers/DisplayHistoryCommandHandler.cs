namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Client.Linq;
    using Persistence;
    using Presentation;

    public class DisplayHistoryCommandHandler : CommandHandler<DisplayHistoryCommand>
    {
        private readonly WalletUi _walletUi;
        private readonly TimeMaster _timeMaster;
        private readonly WalletHistory _walletHistory;
        private readonly DocumentStoreProvider _storeProvider;

        public DisplayHistoryCommandHandler(WalletHistory walletHistory, WalletUi walletUi, TimeMaster timeMaster, DocumentStoreProvider storeProvider)
        {
            _walletHistory = walletHistory;
            _walletUi = walletUi;
            _timeMaster = timeMaster;
            _storeProvider = storeProvider;
        }

        public void Handle(DisplayHistoryCommand command)
        {
            var operations = GetOperations(command);

            _walletUi.DisplayHistory(new History()
            {
                Operations = operations
            }, new HistoryDisplayVerbosity
            {
                Descriptions = command.DisplayDescriptions,
                Tags = command.DisplayTags
            });
        }

        private IList<Operation> GetOperations(DisplayHistoryCommand command)
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                var operationsQuery = _walletHistory.QueryOperations(session);

                if (command.Month == null && command.Monthly)
                {
                    command.Month = new Month(_timeMaster.Today.Year, _timeMaster.Today.Month);
                }

                if (command.Month != null)
                {
                    operationsQuery = operationsQuery.Where(operation => operation.MonthYear == command.Month.GetIndexString());
                }

                if (command.Sources.Any())
                {
                    var operations = new List<Operation>();
                    var tagSources = command.Sources.Where(Tag.IsTagName);
                    foreach (var tagSource in tagSources)
                    {
                        operations.AddRange(operationsQuery.Where(operation => operation.TagNames.Any(tag => tag == tagSource)).OfType<Operation>());
                    }
                    var regularSources = command.Sources.Where(source => !Tag.IsTagName(source));
                    foreach (var regularSource in regularSources)
                    {
                        operations.AddRange(operationsQuery.Where(operation => operation.Sources.Any(source => source == regularSource)).OfType<Operation>());
                    }

                    return operations.OrderBy(operation => operation.When).ToList();
                }

                return operationsQuery.OrderBy(operation => operation.When).OfType<Operation>().ToList();
            }
        }
    }
}