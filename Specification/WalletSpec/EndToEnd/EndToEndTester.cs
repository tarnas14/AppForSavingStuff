namespace Specification.WalletSpec.EndToEnd
{
    using System;
    using System.Linq;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;
    using Tarnas.ConsoleUi;

    class EndToEndTester
    {
        private readonly ConsoleUi _ui;
        private readonly ConsoleMock _consoleMock;
        private readonly Mock<TimeMaster> _timeMasterMock;

        public EndToEndTester()
        {
            _ui = new ConsoleUi();
            _consoleMock = new ConsoleMock();

            var ravenHistory = new RavenDocumentStoreWalletHistory(
                new DocumentStoreProvider
                {
                    RunInMemory = true
                }
            )
            {
                WaitForNonStale = true
            };

            _timeMasterMock = new Mock<TimeMaster>();

            _ui.Subscribe(new WalletMainController(new WalletUi(_consoleMock), ravenHistory, _timeMasterMock.Object), "wallet");
        }

        public EndToEndTester Execute(string userCommandString)
        {
            _ui.UserInput(userCommandString);

            return this;
        }

        public EndToEndTester Execute(params string[] userCommands)
        {
            userCommands.ToList().ForEach(userCommand => Execute(userCommand));

            return this;
        }

        public EndToEndTester SetTime(DateTime time)
        {
            _timeMasterMock.Setup(master => master.Now).Returns(time);
            _timeMasterMock.Setup(master => master.Today).Returns(time);

            return this;
        }

        public void AssertExpectedResult(params string[] expectedOutput)
        {
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }
    }
}
