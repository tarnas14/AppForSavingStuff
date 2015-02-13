﻿namespace Specification.Commands
{
    using System;
    using System.Linq;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class InputCommandTests
    {
        private Mock<TimeMaster> _timeMasterMock;
        private Mock<WalletHistory> _walletHistoryMock;

        [SetUp]
        public void Setup()
        {
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(timeMaster => timeMaster.Now).Returns(DateTime.Now);
            _walletHistoryMock = new Mock<WalletHistory>();
            _walletHistoryMock.Setup(walletHistory => walletHistory.GetBalance(It.IsAny<string>()))
                .Returns(new Moneyz(2));
        }

        [Test]
        public void ShouldStoreAddOperation()
        {
            //given
            const string testSource = "someSource";
            const string testDescription = "test description";
            var testHowMuch = new Moneyz(2);

            var command = new AddCommand
            {
                Source = testSource,
                OperationInput = new OperationInput
                {
                    Description = testDescription,
                    HowMuch = testHowMuch
                }
            };
            var commandHandler = new AddCommandHandler(_walletHistoryMock.Object, _timeMasterMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasGoodOperationData(operation, command.OperationInput) && HasChangeDescribingOperation(operation, command.Source, command.OperationInput))), Times.Once);
        }

        [Test]
        public void ShouldStoreSubtractOperation()
        {
            //given
            const string testSource = "someSource";
            const string testDescription = "test description";
            var testHowMuch = new Moneyz(2);
            var command = new SubtractCommand
            {
                Source = testSource,
                OperationInput = new OperationInput
                {
                    Description = testDescription,
                    HowMuch = testHowMuch
                }
            };
            var commandHandler = new SubtractCommandHandler(_walletHistoryMock.Object, _timeMasterMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasGoodOperationData(operation, command.OperationInput) && HasChangeDescribingOperation(operation, command.Source, command.OperationInput))), Times.Once);
        }

        [Test]
        public void ShouldStoreTransferOperation()
        {
            //given
            const string testSource = "someSource";
            const string testDestination = "someDestination";
            const string testDescription = "test description";
            var testHowMuch = new Moneyz(2);
            var command = new TransferCommand
            {
                Source = testSource,
                Destination = testDestination,
                OperationInput = new OperationInput
                {
                    Description = testDescription,
                    HowMuch = testHowMuch
                }
            };
            var commandHandler = new TransferCommandHandler(_walletHistoryMock.Object, _timeMasterMock.Object);

            //when
            commandHandler.Execute(command);

            //then
            _walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasGoodOperationData(operation, command.OperationInput) && HasChangesDescribingTransfer(operation, command))), Times.Once);
        }

        private bool HasChangesDescribingTransfer(Operation operation, TransferCommand command)
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
                                                   command.OperationInput.HowMuch);
            var secondChangeAddsToDestination = secondChange.Source == command.Destination &&
                                                (secondChange.After - secondChange.Before).Equals(
                                                    command.OperationInput.HowMuch);

            Console.WriteLine("firstChange: {0}, secondChange: {1}", firstChangeRemovesFromSource, secondChangeAddsToDestination);

            return firstChangeRemovesFromSource && secondChangeAddsToDestination;
        }

        private bool HasGoodOperationData(Operation operation, OperationInput input)
        {
            var goodDescription = operation.Description == input.Description;

            Console.WriteLine("description: {0}", operation.Description);

            return goodDescription;
        }

        private bool HasChangeDescribingOperation(Operation operation, string sourceName, OperationInput input)
        {
            var onlyOneChange = operation.Changes.Count == 1;

            if (!onlyOneChange)
            {
                Console.WriteLine("expected 1 change");
                return false;
            }

            var change = operation.Changes.First();
            var addedGoodAmount = (change.After - change.Before).Absolute.Equals(input.HowMuch);
            var properSource = change.Source == sourceName;

            Console.WriteLine("amount: {0}, source: {1}", addedGoodAmount, properSource);

            return properSource && addedGoodAmount;
        }
    }
}
