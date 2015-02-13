namespace Modules.MoneyTracking.CommandHandlers
{
    public class TransferCommandHandler : CommandHandler<TransferCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly TimeMaster _timeMaster;

        public TransferCommandHandler(WalletHistory walletHistory, TimeMaster timeMaster)
        {
            _walletHistory = walletHistory;
            _timeMaster = timeMaster;
        }

        public void Execute(TransferCommand command)
        {
            var operation = new Operation(_timeMaster.Now)
            {
                Description = command.OperationInput.Description,
                Tags = command.OperationInput.Tags
            };

            var before = _walletHistory.GetBalance(command.Source);
            operation.AddChange(command.Source, before, before - command.OperationInput.HowMuch);

            before = _walletHistory.GetBalance(command.Destination);
            operation.AddChange(command.Destination, before, before + command.OperationInput.HowMuch);

            _walletHistory.SaveOperation(operation);
        }
    }
}