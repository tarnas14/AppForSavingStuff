namespace Specification.Commands
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class DisplayHistoryTests
    {
        private Mock<WalletHistory> _walletHistory;
        private ConsoleMock _consoleMock;
        private WalletUi _walletUi;
        private DisplayHistoryCommandHandler _handler;
        private Mock<TimeMaster> _timeMasterMock;


        [SetUp]
        public void Setup()
        {
            _walletHistory = new Mock<WalletHistory>();
            _consoleMock = new ConsoleMock();
            _walletUi = new WalletUi(_consoleMock);
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(mock => mock.Today).Returns(new DateTime(2014, 5, 25));
            _handler = new DisplayHistoryCommandHandler(_walletHistory.Object, _walletUi, _timeMasterMock.Object);
            SetupWalletHistory();
        }

        private void SetupWalletHistory()
        {
            var testDate = new DateTime(2014, 5, 25);
            var monthEarlier = new DateTime(2014, 4, 25);
            var operation = new Operation(monthEarlier);
            var op1 = operation;
            op1.AddChange("mbank", new Moneyz(0), new Moneyz(2.5m));
            var op2 = new Operation(testDate);
            op2.AddChange("mbank", new Moneyz(2.5m), new Moneyz(2.1m));
            var op3 = new Operation(testDate);
            op3.AddChange("getin", new Moneyz(0), new Moneyz(0.01m));
            var op4 = new Operation(testDate);
            op4.AddChange("mbank", new Moneyz(2.1m), new Moneyz(2));
            op4.AddChange("getin", new Moneyz(0.01m), new Moneyz(0.11m));

            _walletHistory.Setup(history => history.GetFullHistory()).Returns(new [] {op1, op2, op3, op4});
            _walletHistory.Setup(history => history.GetForMonth(2014, 5)).Returns(new[] {op2, op3, op4});
        }

        [Test]
        public void ShouldDisplayHistoryOfOperations()
        {
            //given
            var command = new DisplayHistoryCommand();
            var expectedLines = new List<string>
            {
                "    when        where         howMuch  valueAfter",
                string.Empty,
                "    2014-04-25  mbank           +2.50        2.50",
                "    2014-05-25  mbank           -0.40        2.10",
                "    2014-05-25  getin           +0.01        0.01",
                "    2014-05-25  mbank->getin     0.10            ",
                "                mbank           -0.10        2.00",
                "                getin           +0.10        0.11"
            };

            //when
            _handler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        [Test]
        public void ShouldDisplayHistoryOfOperationsForCurrentMonth()
        {
            //given
            var command = new DisplayHistoryCommand
            {
                Monthly = true
            };
            var expectedLines = new List<string>
            {
                "    when        where         howMuch  valueAfter",
                string.Empty,
                "    2014-05-25  mbank           -0.40        2.10",
                "    2014-05-25  getin           +0.01        0.01",
                "    2014-05-25  mbank->getin     0.10            ",
                "                mbank           -0.10        2.00",
                "                getin           +0.10        0.11"
            };

            //when
            _handler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        [Test]
        public void ShouldDisplayHistoryForChosenSource()
        {
            //given
            var command = new DisplayHistoryCommand
            {
                Sources = new[] { "mbank" }
            };

            var expectedLines = new List<string>
            {
                "    when        where         howMuch  valueAfter",
                string.Empty,
                "    2014-04-25  mbank           +2.50        2.50",
                "    2014-05-25  mbank           -0.40        2.10",
                "    2014-05-25  mbank->getin     0.10            ",
                "                mbank           -0.10        2.00",
                "                getin           +0.10        0.11"
            };

            //when
            _handler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }
    }
}
