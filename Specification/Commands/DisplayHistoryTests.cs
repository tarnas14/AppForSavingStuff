﻿namespace Specification.Commands
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class DisplayHistoryTests
    {
        private WalletHistory _walletHistory;
        private ConsoleMock _consoleMock;
        private WalletUi _walletUi;
        private DisplayHistoryCommandHandler _handler;
        private Mock<TimeMaster> _timeMasterMock;


        [SetUp]
        public void Setup()
        {
            _walletHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider(){RunInMemory = true}){WaitForNonStale = true};
            SetupWalletHistory();

            _consoleMock = new ConsoleMock();
            _walletUi = new WalletUi(_consoleMock);

            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(mock => mock.Today).Returns(new DateTime(2014, 5, 25));

            _handler = new DisplayHistoryCommandHandler(_walletHistory, _walletUi, _timeMasterMock.Object);
        }

        private void SetupWalletHistory()
        {
            var testDate = new DateTime(2014, 5, 25);
            var monthEarlier = new DateTime(2014, 4, 25);

            var op1 = new Operation(monthEarlier);
            op1.AddChange("mbank", new Moneyz(2.5m));
            var op2 = new Operation(testDate);
            op2.AddChange("mbank", new Moneyz(-0.4m));
            var op3 = new Operation(testDate);
            op3.AddChange("getin", new Moneyz(0.01m));
            var op4 = new Operation(testDate);
            op4.AddChange("mbank", new Moneyz(-0.1m));
            op4.AddChange("getin", new Moneyz(0.1m));
            var op5 = new Operation(testDate);
            op5.AddChange("src", new Moneyz(69) );

            _walletHistory.SaveOperation(op1);
            _walletHistory.SaveOperation(op2);
            _walletHistory.SaveOperation(op3);
            _walletHistory.SaveOperation(op4);
            _walletHistory.SaveOperation(op5);
        }

        [Test]
        public void ShouldDisplayHistoryOfOperationsWhenMonthlyIsFalseAndMonthNotSet()
        {
            //given
            var command = new DisplayHistoryCommand
            {
                Monthly = false
            };
            var expectedLines = new List<string>
            {
                "    when        where         howMuch  valueAfter",
                string.Empty,
                "    2014-04-25  mbank           +2.50        2.50",
                "    2014-05-25  mbank           -0.40        2.10",
                "    2014-05-25  getin           +0.01        0.01",
                "    2014-05-25  mbank->getin     0.10            ",
                "                mbank           -0.10        2.00",
                "                getin           +0.10        0.10",
                "    2014-05-25  src            +69.00       69.00"
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
                "                getin           +0.10        0.10",
                "    2014-05-25  src            +69.00       69.00"
            };

            //when
            _handler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        [Test]
        public void ShouldDisplayHistoryForSpecifiedMonth()
        {
            //given
            var command = new DisplayHistoryCommand
            {
                Monthly = true,
                Month = Month.FromString("2014-04")
            };
            var expectedLines = new List<string>
            {
                "    when        where  howMuch  valueAfter",
                string.Empty,
                "    2014-04-25  mbank    +2.50        2.50"
            };

            //when
            _handler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        [Test]
        public void ShouldDisplayHistoryForSpecificMonthEvenIfMonthlyIsNotSet()
        {
            //given
            var command = new DisplayHistoryCommand
            {
                Monthly = false,
                Month = Month.FromString("2014-04")
            };
            var expectedLines = new List<string>
            {
                "    when        where  howMuch  valueAfter",
                string.Empty,
                "    2014-04-25  mbank    +2.50        2.50"
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
                "                getin           +0.10        0.10"
            };

            //when
            _handler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        [Test]
        public void ShouldDisplayHistoryForMultipleSources()
        {
            //given
            var command = new DisplayHistoryCommand
            {
                Sources = new[] { "mbank", "src" }
            };

            var expectedLines = new List<string>
            {
                "    when        where         howMuch  valueAfter",
                string.Empty,
                "    2014-04-25  mbank           +2.50        2.50",
                "    2014-05-25  mbank           -0.40        2.10",
                "    2014-05-25  mbank->getin     0.10            ",
                "                mbank           -0.10        2.00",
                "                getin           +0.10        0.10",
                "    2014-05-25  src            +69.00       69.00"
            };

            //when
            _handler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }
    }
}
