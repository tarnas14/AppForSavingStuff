namespace Specification.Commands
{
    using System;
    using System.Linq;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class OperationCommandsTests
    {
        private Mock<TimeMaster> _timeMasterMock;
        private Mock<WalletHistory> _walletHistoryMock;
        private Mock<ReservedWordsStore> _reservedWordsStoreMock;

        [SetUp]
        public void Setup()
        {
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(timeMaster => timeMaster.Now).Returns(DateTime.Now);

            _walletHistoryMock = new Mock<WalletHistory>();
            _walletHistoryMock.Setup(walletHistory => walletHistory.GetBalance(It.IsAny<string>()))
                .Returns(new Moneyz(2));

            _reservedWordsStoreMock = new Mock<ReservedWordsStore>();
            _reservedWordsStoreMock.Setup(mock => mock.IsReserved(It.IsAny<string>())).Returns(false);
        }

        [Test]
        public void ShouldAddOperationWithDateGivenInCommand()
        {
            //given
            const string testSource = "someSource";
            const string testDescription = "test description";
            var testHowMuch = new Moneyz(2);

            var command = new OperationCommand
            {
                Source = testSource,
                Description = testDescription,
                HowMuch = testHowMuch,
                When = DateTime.Now
            };
            var commandHandler = new OperationCommandHandler(_walletHistoryMock.Object, _timeMasterMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasDate(operation, command.When.Value))));
        }

        [Test]
        public void ShouldUseTimeMasterObjectIfDateIsNotGivenInCommand()
        {
            //given
            const string testSource = "someSource";
            const string testDescription = "test description";
            var testHowMuch = new Moneyz(2);

            var command = new OperationCommand
            {
                Source = testSource,
                Description = testDescription,
                HowMuch = testHowMuch
            };
            var commandHandler = new OperationCommandHandler(_walletHistoryMock.Object, _timeMasterMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _timeMasterMock.Verify(mock => mock.Today, Times.Once);
        }

        private bool HasDate(Operation operation, DateTime when)
        {
            return operation.When == when;
        }

        [Test]
        public void ShouldStorePositiveOperation()
        {
            //given
            const string testSource = "someSource";
            const string testDescription = "test description";
            var testHowMuch = new Moneyz(2);

            var command = new OperationCommand
            {
                Source = testSource,
                Description = testDescription,
                HowMuch = testHowMuch
            };
            var commandHandler = new OperationCommandHandler(_walletHistoryMock.Object, _timeMasterMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasGoodOperationData(operation, command) && HasChangeDescribingOperation(operation, command.Source, command))), Times.Once);
        }

        [Test]
        public void ShouldStoreSubtractOperation()
        {
            //given
            const string testSource = "someSource";
            const string testDescription = "test description";
            var testHowMuch = new Moneyz(-2);
            var command = new OperationCommand
            {
                Source = testSource,
                Description = testDescription,
                HowMuch = testHowMuch
            };
            var commandHandler = new OperationCommandHandler(_walletHistoryMock.Object, _timeMasterMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasGoodOperationData(operation, command) && HasChangeDescribingOperation(operation, command.Source, command))), Times.Once);
        }

        [Test]
        public void ShouldStoreTransferOperation()
        {
            //given
            const string testSource = "someSource";
            const string testDestination = "someDestination";
            const string testDescription = "test description";
            var testHowMuch = new Moneyz(2);
            var command = new OperationCommand
            {
                Source = testSource,
                Destination = testDestination,
                Description = testDescription,
                HowMuch = testHowMuch
            };
            var commandHandler = new OperationCommandHandler(_walletHistoryMock.Object, _timeMasterMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasGoodOperationData(operation, command) && HasChangesDescribingTransfer(operation, command))), Times.Once);
        }

        [Test]
        public void ShouldSetupANewSource()
        {
            //given
            const string sourceName = "sourceName";
            var command = new CreateSourceCommand
            {
                Name = sourceName
            };
            var commandHandler = new CreateSourceCommandHandler(_walletHistoryMock.Object, _reservedWordsStoreMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _walletHistoryMock.Verify(mock => mock.CreateSource(sourceName), Times.Once);
        }

        [Test]
        public void ShouldNotCreateSourceWithNameFromReservedWords()
        {
            //given
            _reservedWordsStoreMock.Setup(store => store.IsReserved(It.IsAny<string>())).Returns(true);
            var commandHandler = new CreateSourceCommandHandler(_walletHistoryMock.Object, _reservedWordsStoreMock.Object);
            var command = new CreateSourceCommand
            {
                Name = "testName"
            };

            //when
            TestDelegate result = () => commandHandler.Execute(command);

            //then
            Assert.Throws<WalletException>(result);
        }

        private bool HasChangesDescribingTransfer(Operation operation, OperationCommand command)
        {
            var twoChanges = operation.Changes.Count == 2;

            if (!twoChanges)
            {
                Console.WriteLine("expected 2 changes");
                return false;
            }

            var firstChange = operation.Changes[0];
            var secondChange = operation.Changes[1];

            var firstChangeRemovesFromSource = firstChange.Source == command.Source &&
                                               (firstChange.Before - firstChange.After).Equals(
                                                   command.HowMuch);
            var secondChangeAddsToDestination = secondChange.Source == command.Destination &&
                                                (secondChange.After - secondChange.Before).Equals(
                                                    command.HowMuch);

            Console.WriteLine("firstChange: {0}, secondChange: {1}", firstChangeRemovesFromSource, secondChangeAddsToDestination);

            return firstChangeRemovesFromSource && secondChangeAddsToDestination;
        }

        private bool HasChangeDescribingOperation(Operation operation, string sourceName, OperationCommand command)
        {
            var onlyOneChange = operation.Changes.Count == 1;

            if (!onlyOneChange)
            {
                Console.WriteLine("expected 1 change");
                return false;
            }

            var change = operation.Changes.First();
            var addedGoodAmount = (change.After - change.Before).Equals(command.HowMuch);
            var properSource = change.Source == sourceName;

            Console.WriteLine("amount: {0}, source: {1}", addedGoodAmount, properSource);

            return properSource && addedGoodAmount;
        }

        private bool HasGoodOperationData(Operation operation, OperationCommand command)
        {
            var goodDescription = operation.Description == command.Description;

            Console.WriteLine("description: {0}", operation.Description);

            return goodDescription;
        }
    }
}
