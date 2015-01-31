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
            var consoleUi = new ConsoleUi();
            var wallet = new WalletMainController(new WalletUi(new SystemConsole()), new Wallet(new RavenDocumentStoreWalletHistory(new DocumentStoreProvider()), new SystemClockTimeMaster()));
            consoleUi.Subscribe(wallet, "wallet");

            new InputLoop(consoleUi).Loop();
        }
    }
}
