namespace Modules.MoneyTracking.CommandHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Persistence;
    using Raven.Client;
    using SourceNameValidation;

    public class OperationCommandHandler : CommandHandler<OperationCommand>
    {
        private readonly SourceNameValidator _sourceNameValidator;
        private readonly BagOfRavenMagic _ravenMagic;

        public OperationCommandHandler(SourceNameValidator sourceNameValidator, BagOfRavenMagic ravenMagic)
        {
            _sourceNameValidator = sourceNameValidator;
            _ravenMagic = ravenMagic;
        }

        public void Handle(OperationCommand command)
        {
            _sourceNameValidator.CheckIfValid(command.Source);

            var operation = new Operation(command.When)
            {
                Description = command.Description,
                TagStrings = command.Tags.Select(tag => tag.Value).ToList()
            };

            if (HasDestination(command))
            {
                AddTransferChanges(operation, command);
            }
            else
            {
                StandardOperation(operation, command);
            }

            SaveTags(command.Tags);
            SaveOperation(operation);
        }

        private void SaveTags(IEnumerable<Tag> tags)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                foreach (var newTag in tags.Distinct(new Tag.Comparer()))
                {
                    if (session.Query<Tag>().Where(storedTag => storedTag.Value == newTag.Value).ToList().Any())
                    {
                        continue;
                    }
                    session.Store(newTag);
                }
                session.SaveChanges();
            }
        }

        private bool HasDestination(OperationCommand command)
        {
            return !string.IsNullOrEmpty(command.Destination);
        }

        private void StandardOperation(Operation operation, OperationCommand command)
        {
            operation.AddChange(command.Source, command.HowMuch);
        }

        private void AddTransferChanges(Operation operation, OperationCommand command)
        {
            operation.AddChange(command.Source, -command.HowMuch);

            operation.AddChange(command.Destination, command.HowMuch);
        }

        private void SaveOperation(Operation toSave)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                AdditionalInformationToChanges(toSave.Changes, toSave.When, session);
                session.Store(toSave);
                session.SaveChanges();
            }
        }

        private void AdditionalInformationToChanges(IEnumerable<Change> changes, DateTime when, IDocumentSession session)
        {
            foreach (var change in changes)
            {
                var sourceBalance = GetBalanceAt(change.Source, when, session);

                change.Before = sourceBalance;
                change.After = sourceBalance + change.Difference;

                AdjustLaterOperationsOnSourceIfNecessary(when, change.Source, change.Difference, session);
            }
        }

        private Moneyz GetBalanceAt(string sourceName, DateTime when, IDocumentSession session)
        {
            var operationsBefore = _ravenMagic.WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>()).OfType<Operation>().ToList();
            var latestOperationBefore = operationsBefore.Where(operation => operation.When <= when && operation.Changes.Any(change => change.Source == sourceName)).OrderByDescending(operation => operation.When).FirstOrDefault();

            if (latestOperationBefore == null)
            {
                return new Moneyz(0);
            }

            var changeForSource = latestOperationBefore.Changes.FirstOrDefault(change => change.Source == sourceName);

            if (changeForSource == null)
            {
                return new Moneyz(0);
            }

            return changeForSource.After;
        }

        private void AdjustLaterOperationsOnSourceIfNecessary(DateTime when, string sourceName, Moneyz difference, IDocumentSession session)
        {
            var laterOperations =
                _ravenMagic.WaitForQueryIfNecessary(session.Query<Operations_ByMonthYear.Result, Operations_ByMonthYear>())
                    .Where(operation => operation.When > when)
                    .OfType<Operation>().ToList();

            laterOperations.ForEach(operation => operation.Changes.Where(change => change.Source == sourceName).ToList().ForEach(
                change =>
                {
                    change.Before += difference;
                    change.After += difference;
                }));
        }
    }
}