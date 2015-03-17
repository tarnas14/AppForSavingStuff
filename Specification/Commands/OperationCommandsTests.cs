namespace Specification.Commands
{
    using System;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class OperationCommandsTests
    {
        private Mock<TimeMaster> _timeMasterMock;
        private Mock<ReservedWordsStore> _reservedWordsStoreMock;
        private WalletHistory _walletHistory;
        private OperationCommandHandler _commandHandler;
        private const string TestSource = "testSource";
        private const string TestDestination = "testDestination";

        [SetUp]
        public void Setup()
        {
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(timeMaster => timeMaster.Now).Returns(DateTime.Now);

            _walletHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider() {RunInMemory = true})
            {
                WaitForNonStale = true
            };

            _reservedWordsStoreMock = new Mock<ReservedWordsStore>();
            _reservedWordsStoreMock.Setup(mock => mock.IsReserved(It.IsAny<string>())).Returns(false);

            _commandHandler = new OperationCommandHandler(_walletHistory, _timeMasterMock.Object, _reservedWordsStoreMock.Object);
        }

        [Test]
        public void ShouldNotAllowReservedSource()
        {
            //given
            var command = new OperationCommand
            {
                Source = "reserved"
            };
            _reservedWordsStoreMock.Setup(mock => mock.IsReserved(It.IsAny<string>())).Returns(true);

            //when
            TestDelegate savingOperationWithRerservedSourcename = () => _commandHandler.Execute(command);

            //then
            Assert.That(savingOperationWithRerservedSourcename, Throws.Exception.TypeOf<SourceNameIsReservedException>());
        }

        [Test]
        public void ShouldAddOperationWithDateGivenInCommand()
        {
            //given
            var command = new OperationCommand
            {
                Source = TestSource,
                HowMuch = new Moneyz(2),
                When = DateTime.Now
            };
            var walletHistoryMock = new Mock<WalletHistory>();
            walletHistoryMock.Setup(history => history.GetBalance(It.IsAny<string>())).Returns(new Moneyz(0));
            var commandHandler = new OperationCommandHandler(walletHistoryMock.Object, _timeMasterMock.Object, Mock.Of<ReservedWordsStore>());

            //when
            commandHandler.Execute(command);

            //then
            walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasDate(operation, command.When.Value))));
        }

        private bool HasDate(Operation operation, DateTime when)
        {
            return operation.When == when;
        }

        [Test]
        public void ShouldUseTimeMasterObjectIfDateIsNotGivenInCommand()
        {
            //given
            var command = new OperationCommand
            {
                HowMuch = new Moneyz(2),
                Source = TestSource
            };

            //when
            _commandHandler.Execute(command);

            //then
            _timeMasterMock.Verify(mock => mock.Today, Times.Once);
        }

        [Test]
        public void ShouldStorePositiveOperation()
        {
            //given
            var testHowMuch = new Moneyz(2);

            var command = new OperationCommand
            {
                Source = TestSource,
                HowMuch = testHowMuch
            };

            //when
            _commandHandler.Execute(command);

            //then
            var balance = _walletHistory.GetBalance(TestSource);
            Assert.That(balance, Is.EqualTo(testHowMuch));
        }

        [Test]
        public void ShouldStoreSubtractOperation()
        {
            //given
            var testHowMuch = new Moneyz(-2);
            var command = new OperationCommand
            {
                Source = TestSource,
                HowMuch = testHowMuch
            };

            //when
            _commandHandler.Execute(command);

            //then
            var balance = _walletHistory.GetBalance(TestSource);
            Assert.That(balance, Is.EqualTo(testHowMuch));
        }

        [Test]
        public void ShouldStoreTransferOperation()
        {
            //given
            const int howMuch = 2;
            var command = new OperationCommand
            {
                Source = TestSource,
                Destination = TestDestination,
                HowMuch = new Moneyz(howMuch)
            };

            //when
            _commandHandler.Execute(command);

            //then
            var sourceBalance = _walletHistory.GetBalance(TestSource);
            Assert.That(sourceBalance, Is.EqualTo(new Moneyz(-howMuch)));

            var destinationBalance = _walletHistory.GetBalance(TestDestination);
            Assert.That(destinationBalance, Is.EqualTo(new Moneyz(howMuch)));
        }
    }
}
