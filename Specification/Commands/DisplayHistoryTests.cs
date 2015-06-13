namespace Specification.Commands
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Modules.MoneyTracking.SourceNameValidation;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class DisplayHistoryTests
    {
        private ConsoleMock _consoleMock;
        private WalletUi _walletUi;
        private DisplayHistoryCommandHandler _handler;
        private Mock<TimeMaster> _timeMasterMock;


        [SetUp]
        public void Setup()
        {
            var documentStoreProvider = new DocumentStoreProvider(){RunInMemory = true};
            SetupWalletHistory(documentStoreProvider);

            _consoleMock = new ConsoleMock();
            _walletUi = new WalletUi(_consoleMock);

            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(mock => mock.Today).Returns(new DateTime(2014, 5, 25));

            _handler = new DisplayHistoryCommandHandler(new StandardBagOfRavenMagic(documentStoreProvider), _walletUi, _timeMasterMock.Object);
        }

        private void SetupWalletHistory(DocumentStoreProvider documentStoreProvider)
        {
            var testDate = new DateTime(2014, 5, 25);
            var monthEarlier = new DateTime(2014, 4, 25);

            var command1 = new OperationCommand
            {
                When = monthEarlier,
                Source = "mbank",
                HowMuch = new Moneyz(2.5m)
            };

            var command2 = new OperationCommand
            {
                When = testDate,
                Source = "mbank",
                HowMuch = new Moneyz(-0.4m)
            };

            var command3 = new OperationCommand
            {
                When = testDate,
                Source = "getin",
                HowMuch = new Moneyz(0.01m)
            };

            var command4 = new OperationCommand
            {
                When = testDate,
                Source = "mbank",
                Destination = "getin",
                HowMuch = new Moneyz(0.1m)
            };

            var command5 = new OperationCommand
            {
                When = testDate,
                Source = "src",
                HowMuch = new Moneyz(69)
            };

            var handler = new OperationCommandHandler(Mock.Of<SourceNameValidator>(),
                new StandardBagOfRavenMagic(documentStoreProvider) { WaitForNonStale = true });

            handler.Handle(command1);
            handler.Handle(command2);
            handler.Handle(command3);
            handler.Handle(command4);
            handler.Handle(command5);
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
                "                getin           +0.10        0.11",
                "    2014-05-25  src            +69.00       69.00"
            };

            //when
            _handler.Handle(command);

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
                "                getin           +0.10        0.11",
                "    2014-05-25  src            +69.00       69.00"
            };

            //when
            _handler.Handle(command);

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
            _handler.Handle(command);

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
            _handler.Handle(command);

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
            _handler.Handle(command);

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
                "                getin           +0.10        0.11",
                "    2014-05-25  src            +69.00       69.00"
            };

            //when
            _handler.Handle(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }
    }
}
