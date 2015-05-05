namespace Modules.MoneyTracking.CommandHandlers
{
    using SourceNameValidation;

    public class OperationCommandHandler : CommandHandler<OperationCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly SourceNameValidator _sourceNameValidator;

        public OperationCommandHandler(WalletHistory walletHistory, SourceNameValidator sourceNameValidator)
        {
            _walletHistory = walletHistory;
            _sourceNameValidator = sourceNameValidator;
        }

        public void Execute(OperationCommand command)
        {
            _sourceNameValidator.CheckIfValid(command.Source);

            var operation = new Operation(command.When)
            {
                Description = command.Description,
                Tags = command.Tags
            };

            if (HasDestination(command))
            {
                AddTransferChanges(operation, command);
            }
            else
            {
                StandardOperation(operation, command);
            }

            _walletHistory.SaveOperation(operation);
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

        private bool HasDestination(OperationCommand command)
        {
            return !string.IsNullOrEmpty(command.Destination);
        }
    }
}