namespace Application
{
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Modules.MoneyTracking.SourceNameValidation;
    using Tarnas.ConsoleUi;

    class Program
    {
        static void Main(string[] args)
        {
            var consoleUi = new ConsoleUi();

            var documentStoreProvider = new DocumentStoreProvider();
            var ravenDocumentStoreWalletHistory = new RavenDocumentStoreWalletHistory(documentStoreProvider);
            var ravenMagic = new StandardBagOfRavenMagic(documentStoreProvider);

            var systemClockTimeMaster = new SystemClockTimeMaster();
            var reservedWordsStore = new MemoryListSourceNameValidator();
            reservedWordsStore.RestrictWord("tags");

            var wallet = new WalletMainController(new WalletUi(new SystemConsole()), ravenDocumentStoreWalletHistory, systemClockTimeMaster, reservedWordsStore, ravenMagic);
            consoleUi.Subscribe(wallet, "wallet");

            new InputLoop(consoleUi).Loop();
        }
    }
}
