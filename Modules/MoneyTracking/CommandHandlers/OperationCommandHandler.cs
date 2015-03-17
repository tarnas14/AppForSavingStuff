namespace Modules.MoneyTracking.CommandHandlers
{
    using Persistence;

    public class OperationCommandHandler : CommandHandler<OperationCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly TimeMaster _timeMaster;
        private readonly ReservedWordsStore _reservedWordsStore;

        public OperationCommandHandler(WalletHistory walletHistory, TimeMaster timeMaster, ReservedWordsStore reservedWordsStore)
        {
            _walletHistory = walletHistory;
            _timeMaster = timeMaster;
            _reservedWordsStore = reservedWordsStore;
        }

        public void Execute(OperationCommand command)
        {
            CheckIfSourceNameIsReserved(command.Source);

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

        private void CheckIfSourceNameIsReserved(string source)
        {
            if (_reservedWordsStore.IsReserved(source))
            {
                throw new SourceNameIsReservedException();
            }
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