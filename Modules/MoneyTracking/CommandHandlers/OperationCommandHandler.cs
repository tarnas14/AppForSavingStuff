namespace Modules.MoneyTracking.CommandHandlers
{
    public class OperationCommandHandler : CommandHandler<OperationCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly TimeMaster _timeMaster;

        public OperationCommandHandler(WalletHistory walletHistory, TimeMaster timeMaster)
        {
            _walletHistory = walletHistory;
            _timeMaster = timeMaster;
        }

        public void Execute(OperationCommand command)
        {
            var when = command.When ?? _timeMaster.Today;

            var operation = new Operation(when)
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
            var before = _walletHistory.GetBalance(command.Source);
            operation.AddChange(command.Source, before, before + command.HowMuch);
        }

        private void AddTransferChanges(Operation operation, OperationCommand command)
        {
            var before = _walletHistory.GetBalance(command.Source);
            operation.AddChange(command.Source, before, before - command.HowMuch);

            before = _walletHistory.GetBalance(command.Destination);
            operation.AddChange(command.Destination, before, before + command.HowMuch);
        }

        private bool HasDestination(OperationCommand command)
        {
            return !string.IsNullOrEmpty(command.Destination);
        }
    }
}