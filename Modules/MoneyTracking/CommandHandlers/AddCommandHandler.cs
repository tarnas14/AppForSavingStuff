namespace Modules.MoneyTracking.CommandHandlers
{
    public class AddCommandHandler : CommandHandler<AddCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly TimeMaster _timeMaster;

        public AddCommandHandler(WalletHistory walletHistory, TimeMaster timeMaster)
        {
            _walletHistory = walletHistory;
            _timeMaster = timeMaster;
        }

        public void Execute(AddCommand command)
        {
            var before = _walletHistory.GetBalance(command.Source);
            var operation = new Operation(_timeMaster.Now)
            {
                Description = command.OperationInput.Description,
                Tags = command.OperationInput.Tags
            };
            operation.AddChange(command.Source, before, before + command.OperationInput.HowMuch);

            _walletHistory.SaveOperation(operation);
        }
    }
}