namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
    using System.Linq;
    using Persistence;
    using Raven.Client.Linq;
    using Presentation;

    public class DisplayHistoryCommandHandler : CommandHandler<DisplayHistoryCommand>
    {
        private readonly BagOfRavenMagic _bagOfRavenMagic;
        private readonly WalletUi _walletUi;
        private readonly TimeMaster _timeMaster;

        public DisplayHistoryCommandHandler(BagOfRavenMagic bagOfRavenMagic, WalletUi walletUi, TimeMaster timeMaster)
        {
            _bagOfRavenMagic = bagOfRavenMagic;
            _walletUi = walletUi;
            _timeMaster = timeMaster;
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
            using (var session = _bagOfRavenMagic.Store.OpenSession())
            {
                var operationsQuery = _bagOfRavenMagic.WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>());

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