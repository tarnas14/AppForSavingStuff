namespace Modules.MoneyTracking
{
    public class WalletUi
    {
        private readonly Console _console;
        private const string Tab = "    ";

        public WalletUi(Console console)
        {
            _console = console;
        }

        public void DisplayBalance(string sourceName, Moneyz balance)
        {
            _console.WriteLine(string.Format("{0}{1}: {2}", Tab, sourceName, balance));
        }

        public void DisplayError(WalletException exception)
        {
            _console.WriteLine(exception.Message);
        }
    }
}