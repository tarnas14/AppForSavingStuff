namespace Application
{
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Tarnas.ConsoleUi;

    class Program
    {
        static void Main(string[] args)
        {
            var consoleUi = new ConsoleUi(new CleverFactory());
            var wallet = new WalletMainController(new WalletUi(new SystemConsole()), new Wallet(new RavenDocumentStoreWalletHistory(new DocumentStoreProvider()), new SystemClockTimeMaster()));
            consoleUi.Subscribe(wallet, "wallet");

            string input = string.Empty;
            while (input != "/quit")
            {
                input = System.Console.ReadLine();
                consoleUi.UserInput(input);
            }
        }
    }
}
