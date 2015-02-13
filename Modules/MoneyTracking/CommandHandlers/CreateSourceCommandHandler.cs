namespace Modules.MoneyTracking.CommandHandlers
{
    public class CreateSourceCommandHandler : CommandHandler<CreateSourceCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly ReservedWordsStore _reservedWordsStore;

        public CreateSourceCommandHandler(WalletHistory walletHistory, ReservedWordsStore reservedWordsStore)
        {
            _walletHistory = walletHistory;
            _reservedWordsStore = reservedWordsStore;
        }

        public void Execute(CreateSourceCommand command)
        {
            if (_reservedWordsStore.IsReserved(command.Name))
            {
                var errorMessage = string.Format("'{0}' is a reserved word and cannot be used as a source name.", command.Name);

                throw new WalletException(errorMessage);
            }

            _walletHistory.CreateSource(command.Name);
        }
    }
}