namespace Modules.MoneyTracking.CommandHandlers
{
    using System;

    public class SubtractCommandHandler : CommandHandler<SubtractCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly TimeMaster _timeMaster;

        public SubtractCommandHandler(WalletHistory walletHistory, TimeMaster timeMaster)
        {
            _walletHistory = walletHistory;
            _timeMaster = timeMaster;
        }

        public void Execute(SubtractCommand command)
        {
            var before = _walletHistory.GetBalance(command.Source);
            var operation = new Operation(_timeMaster.Now)
            {
                Description = command.OperationInput.Description,
                Tags = command.OperationInput.Tags
            };
            operation.AddChange(command.Source, before, before - command.OperationInput.HowMuch);

            _walletHistory.SaveOperation(operation);
        }
    }
}