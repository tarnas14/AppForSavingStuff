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
            _walletHistoryMock.Verify(mock => mock.SaveOperation(It.Is<Operation>(operation => HasGoodOperationData(operation, command) && HasChangeDescribingAddition(operation, command))), Times.Once);
        }

        private bool HasGoodOperationData(Operation operation, AddCommand command)
        {
            var goodDescription = operation.Description == command.OperationInput.Description;

            Console.WriteLine("description: {0}", operation.Description);

            return goodDescription;
        }

        private bool HasChangeDescribingAddition(Operation operation, AddCommand command)
        {
            var onlyOneChange = operation.Changes.Count == 1;

            if (!onlyOneChange)
            {
                Console.WriteLine("expected 1 change");
                return false;
            }

            var change = operation.Changes.First();
            var addedGoodAmount = (change.After - change.Before).Equals(command.OperationInput.HowMuch);
            var properSource = change.Source == command.Source;

            Console.WriteLine("amount: {0}, source: {1}", addedGoodAmount, properSource);

            return properSource && addedGoodAmount;
        }
    }
}
