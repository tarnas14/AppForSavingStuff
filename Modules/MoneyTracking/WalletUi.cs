namespace Modules.MoneyTracking
{
    public interface WalletUi
    {
        void DisplayBalance(string sourceName, Moneyz balance);
        void DisplayError(WalletException exception);
    }
}