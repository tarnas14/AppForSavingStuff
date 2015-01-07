namespace Modules.MoneyTracking
{
    using Ui;

    public class WalletMainController : Subscriber
    {
        private readonly WalletUi _walletUi;

        public WalletMainController(WalletUi walletUi)
        {
            _walletUi = walletUi;
        }

        public void Execute(UserCommand userCommand)
        {
            throw new System.NotImplementedException();
        }
    }
}