namespace Specification.WalletSpec
{
    using System;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using Tarnas.ConsoleUi;

    class EndToEndCommandWalletTests : EndToEndWalletTests
    {
        public override void Setup()
        {
            Now = new DateTime(2015, 5, 24);
            Ui = new ConsoleUi();
            TimeMasterMock = new Mock<TimeMaster>();
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(Now);
            TimeMasterMock.SetupGet(mock => mock.Today).Returns(Now);
            var ravenHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider() { RunInMemory = true })
            {
                WaitForNonStale = true
            };
            ConsoleMock = new ConsoleMock();
            var walletController = new WalletCommandController(new WalletUi(ConsoleMock), ravenHistory, TimeMasterMock.Object);
            Ui.Subscribe(walletController, "wallet");
        }
    }
}
