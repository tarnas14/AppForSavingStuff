namespace Specification.Commands
{
    using System;
    using System.Linq;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.SourceNameValidation;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class OperationCommandsTests
    {
        private Mock<TimeMaster> _timeMasterMock;
        private Mock<SourceNameValidator> _reservedWordsStoreMock;
        private WalletHistory _walletHistory;
        private OperationCommandHandler _commandHandler;
        private const string TestSource = "testSource";
        private const string TestDestination = "testDestination";

        [SetUp]
        public void Setup()
        {
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(timeMaster => timeMaster.Now).Returns(DateTime.Now);

            var documentStoreProvider = new DocumentStoreProvider() {RunInMemory = true};
            _walletHistory = new RavenDocumentStoreWalletHistory(documentStoreProvider)
            {
                WaitForNonStale = true
            };

            var bagOfRavenMagic = new StandardBagOfRavenMagic(documentStoreProvider){WaitForNonStale = true};

            _reservedWordsStoreMock = new Mock<SourceNameValidator>();

            _commandHandler = new OperationCommandHandler(_reservedWordsStoreMock.Object, bagOfRavenMagic);
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

            //when
            _commandHandler.Execute(command);

            //then
            var operation = _walletHistory.GetFullHistory().First();
            Assert.That(operation.When, Is.EqualTo(command.When));
        }

        [Test]
        public void ShouldStorePositiveOperation()
        {
            //given
            var testHowMuch = new Moneyz(2);

            var command = new OperationCommand
            {
                Source = TestSource,
                HowMuch = testHowMuch,
                When = DateTime.Now
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
                HowMuch = testHowMuch,
                When = DateTime.Now
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
                HowMuch = new Moneyz(howMuch),
                When = DateTime.Now
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
