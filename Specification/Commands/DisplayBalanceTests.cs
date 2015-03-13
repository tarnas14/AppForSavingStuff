namespace Specification.Commands
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using NUnit.Framework;

    [TestFixture]
    class DisplayBalanceTests
    {
        private ConsoleMock _consoleMock;
        private DisplayBalanceCommandHandler _commandHandler;
        private WalletHistory _walletHistory;

        [SetUp]
        public void Setup()
        {   
            _consoleMock = new ConsoleMock();

            _walletHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider(){RunInMemory = true}){WaitForNonStale = true};
            _walletHistory.CreateSource("source1");
            _walletHistory.CreateSource("source2");

            _commandHandler = new DisplayBalanceCommandHandler(_walletHistory, new WalletUi(_consoleMock));
        }

        [Test]
        public void ShouldDisplayBalanceOfASingleSource()
        {
            //given
            const string sourceName = "source1";
            _walletHistory.SaveOperation(new Operation(DateTime.Now).AddChange(sourceName, new Moneyz(2)));
            var command = new DisplayBalanceCommand
            {
                Sources = new[] { sourceName }
            };
            var expectedOutput = new List<string>
            {
                "    source1: 2.00"
            };

            //when
            _commandHandler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayBalanceOfMultipleSources()
        {
            //given
            _walletHistory.SaveOperation(new Operation(DateTime.Now).AddChange("source1", new Moneyz(2)));
            _walletHistory.SaveOperation(new Operation(DateTime.Now).AddChange("source2", new Moneyz(12)));
            var command = new DisplayBalanceCommand
            {
                Sources = new List<string> { "source1", "source2" }
            };
            
            var expectedOutput = new List<string>
            {
                "    source1:  2.00",
                "    source2: 12.00",
                "           : 14.00"
            };

            //when
            _commandHandler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayAllSourceBalancesWhenNoSourcesSpecified()
        {
            //given
            _walletHistory.SaveOperation(new Operation(DateTime.Now).AddChange("source1", new Moneyz(2)));
            _walletHistory.SaveOperation(new Operation(DateTime.Now).AddChange("source2", new Moneyz(12)));
            var command = new DisplayBalanceCommand
            {
                Sources = new List<string>()
            };

            var expectedOutput = new List<string>
            {
                "    source1:  2.00",
                "    source2: 12.00",
                "           : 14.00"
            };

            //when
            _commandHandler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }
    }
}
