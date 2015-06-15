namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
    using System.Linq;
    using Persistence;
    using Presentation;
    using Raven.Abstractions.Extensions;

    public class DisplayBalanceCommandHandler : CommandHandler<DisplayBalanceCommand>
    {
        private readonly WalletUi _walletUi;
        private readonly BagOfRavenMagic _ravenMagic;

        public DisplayBalanceCommandHandler(WalletUi walletUi, BagOfRavenMagic ravenMagic)
        {
            _walletUi = walletUi;
            _ravenMagic = ravenMagic;
        }

        public void Handle(DisplayBalanceCommand command)
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

            balancesToDisplay.DisplaySum = AllOfTheSameType(command.Sources) && NoTagsInSources(command.Sources);

            _walletUi.DisplayBalance(balancesToDisplay);
        }

        private bool NoTagsInSources(IEnumerable<string> sources)
        {
            return !sources.Any(Tag.IsTagName);
        }

        private void CheckIfAllSourcesAreOfTheSameType(IEnumerable<string> sources)
        {
            if (!AllOfTheSameType(sources))
            {
                throw new WalletException("Cannot mix sources and tags in balance command.");
            }
        }

        private bool AllOfTheSameType(IEnumerable<string> sources)
        {
            var tagSources = sources.Count(Tag.IsTagName);

            if (tagSources == 0)
            {
                return true;
            }

            return tagSources == sources.Count();
        }

        private Moneyz GetBalance(string sourceName, Month month)
        {
            if (month == null)
            {
                var source = GetSourceByName(sourceName);

                if (source == null)
                {
                    throw new SourceDoesNotExistException(sourceName);
                }

                return source.Balance;
            }

            return GetSourceBalanceForMonth(sourceName, month);
        }

        private Source GetSourceByName(string sourceName)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                IList<Source> sources = new List<Source>();

                if (Tag.IsTagName(sourceName))
                {
                    sources.Add(GetSourceFromTag(sourceName));
                }
                else
                {
                    sources =
                        _ravenMagic.WaitForQueryIfNecessary(session.Query<Source, Sources_ByChangesInOperations>())
                        .Where(src => src.Name == sourceName)
                        .ToList();
                }

                if (sources.Count == 1)
                {
                    return sources.First();
                }

                return null;
            }
        }

        private Source GetSourceFromTag(string tagName)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var tagOperations = _ravenMagic.WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>()).Where(operation => operation.TagNames.Any(tagString => tagString == tagName)).OfType<Operation>().ToList();

                var changes = tagOperations.SelectMany(operation => operation.Changes);

                return new Source
                {
                    Name = tagName,
                    Balance = changes.Aggregate(new Moneyz(0), (money, change) => money + change.Difference)
                };
            }
        }

        private bool ShouldDisplayAllSources(DisplayBalanceCommand command)
        {
            return !command.Sources.Any();
        }

        private void LoadAllSources(Balances balancesToDisplay)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var sources = _ravenMagic.WaitForQueryIfNecessary(session.Query<Source, Sources_ByChangesInOperations>()).ToList();

                sources.ForEach(balancesToDisplay.AddBalance);
            }
        }

        public Moneyz GetSourceBalanceForMonth(string sourceName, Month month)
        {
            if (Tag.IsTagName(sourceName))
            {
                var tagOperationsThisMonth = GetTagHistoryForThisMonth(sourceName, month);
                var changesForTagThisMonth = tagOperationsThisMonth.SelectMany(operation => operation.Changes);

                var tagMoneyBalanceForMonth = changesForTagThisMonth.Sum(change => change.Difference);

                return tagMoneyBalanceForMonth;
            }

            using (var session = _ravenMagic.Store.OpenSession())
            {
                var monthHistory = _ravenMagic.WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>())
                    .Where(operation => operation.MonthYear == month.GetIndexString())
                    .OrderBy(result => result.When)
                    .OfType<Operation>()
                    .ToList();

                var changesOnSourceThisMonth =
                    monthHistory.SelectMany(operation => operation.Changes.Where(change => change.Source == sourceName));

                var stateBeforeThisMonth = changesOnSourceThisMonth.First().Before;
                var lastChangeInThisMonth = changesOnSourceThisMonth.Last().After;

                return lastChangeInThisMonth - stateBeforeThisMonth;
            }
        }

        public IList<Operation> GetTagHistoryForThisMonth(string tagName, Month month)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operations = _ravenMagic.WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>())
                .Where(result => result.MonthYear == month.GetIndexString() && result.TagNames.Any(tag => tag == tagName))
                .OrderBy(result => result.When).OfType<Operation>().ToList();

                LegacyDataMagic.AddDifferencesToChanges(operations.SelectMany(operation => operation.Changes));

                return operations;
            }
        }
    }
}