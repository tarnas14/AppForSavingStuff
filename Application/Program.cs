namespace Application
{
    using System;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Ui;

    class Program
    {
        static void Main(string[] args)
        {
            var consoleUi = new ConsoleUi(new CleverFactory());
            var wallet = new WalletMainController(new WalletOutboundUi(), new Wallet(new RavenDocumentStoreWalletHistory(new DocumentStoreProvider()), new SystemClockTimeMaster()));
            consoleUi.Subscribe(wallet, "wallet");

            string input = string.Empty;
            while (input != "/quit")
            {
                input = Console.ReadLine();
                consoleUi.UserInput(input);
            }
        }
    }
}
