namespace Specification.WalletSpec.EndToEnd
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;
    using Tarnas.ConsoleUi;

    [TestFixture]
    class EndToEndBaseFixture
    {
        protected ConsoleUi Ui;
        protected Mock<TimeMaster> TimeMasterMock;
        protected ConsoleMock ConsoleMock;
        public DateTime Now { get; set; }

        [SetUp]
        public virtual void Setup()
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
            var walletMainController = new WalletMainController(new WalletUi(ConsoleMock), new Wallet(ravenHistory, TimeMasterMock.Object), ravenHistory, TimeMasterMock.Object);
            Ui.Subscribe(walletMainController, "wallet");
        }

        protected void EndToEndTest(IEnumerable<string> userCommands, IList<string> expectedOutput)
        {
            //given

            //when
            userCommands.ToList().ForEach(Ui.UserInput);

            //then
            ConsoleMock.Lines.ToList().ForEach(System.Console.WriteLine);
            Assert.That(ConsoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }
    }
}
