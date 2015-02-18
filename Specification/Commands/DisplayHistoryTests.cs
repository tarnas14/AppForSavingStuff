namespace Specification.Commands
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;
    using Tarnas.ConsoleUi;
    using Console = Tarnas.ConsoleUi.Console;

    [TestFixture]
    class DisplayHistoryTests
    {
        private Mock<WalletHistory> _walletHistory;
        private ConsoleMock _consoleMock;
        private WalletUi _walletUi;


        [SetUp]
        public void Setup()
        {
            _walletHistory = new Mock<WalletHistory>();
            _consoleMock = new ConsoleMock();
            _walletUi = new WalletUi(_consoleMock);
        }

        [Test]
        public void ShouldDisplayHistoryOfBasicOperations()
        {
            //given
            var command = new DisplayHistoryCommand();
            var handler = new DisplayHistoryCommandHandler(_walletHistory.Object, _walletUi);
            _walletHistory.Setup(history => history.GetFullHistory()).Returns(HistoryDisplayTestData);
            var expectedLines = new List<string>
            {
                "    when        where  howMuch  valueAfter",
                string.Empty,
                "    2014-05-25  mbank    +2.50        2.50",
                "    2014-05-25  mbank    -0.40        2.10",
                "    2014-05-25  getin    +0.01        0.01"
            };

            //when
            handler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        private IList<Operation> HistoryDisplayTestData()
        {
            var testDate = new DateTime(2014, 5, 25);
            var op1 = new Operation(testDate);
            op1.AddChange("mbank", new Moneyz(0), new Moneyz(2.5m));
            var op2 = new Operation(testDate);
            op2.AddChange("mbank", new Moneyz(2.5m), new Moneyz(2.1m));
            var op3 = new Operation(testDate);
            op3.AddChange("getin", new Moneyz(0), new Moneyz(0.01m));
            
            return new[]
            {
                op1, op2, op3
            };
        }
    }
}
