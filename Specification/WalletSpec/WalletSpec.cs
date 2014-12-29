namespace Specification.WalletSpec
{
    using System;
    using System.Linq;
    using Modules;
    using Modules.MoneyTracking;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class WalletSpec
    {
        private const string TestSourceName = "testSource";
        private Wallet _wallet;
        private Mock<TimeMaster> _timeMasterMock;

        [SetUp]
        public void Setup()
        {
            _timeMasterMock = new Mock<TimeMaster>();
            _wallet = new Wallet(new InMemoryWalletHistory(), new OperationFactory(_timeMasterMock.Object), _timeMasterMock.Object);
        }

        [Test]
        public void ShouldAddResourcesToSource()
        {
            //given
            var howMuch = new Moneyz(6.66m);

            //when
            _wallet.Add(TestSourceName, howMuch);
            var currentBalance = _wallet.GetBalance(TestSourceName);

            //then
            Assert.That(currentBalance, Is.EqualTo(howMuch));
        }

        [Test]
        public void ShouldSubtractResourcesFromSource()
        {
            //given
            var howMuch = new Moneyz(6.66m);

            //when
            _wallet.Subtract(TestSourceName, howMuch);
            var currentBalance = _wallet.GetBalance(TestSourceName);

            //then
            Assert.That(currentBalance, Is.EqualTo(new Moneyz(-6.66m)));
        }

        [Test]
        public void ShouldTransferResourcesFromOneSourceToTheOther()
        {
            //given
            _wallet.Add("source", new Moneyz(5));
            _wallet.Add("destination", new Moneyz(2));

            //when
            _wallet.Transfer("source", "destination", new Moneyz(2));
            var sourceBalance = _wallet.GetBalance("source");
            var destinationBalance = _wallet.GetBalance("destination");

            //then
            Assert.That(sourceBalance, Is.EqualTo(new Moneyz(3)));
            Assert.That(destinationBalance, Is.EqualTo(new Moneyz(4)));
        }

        [Test]
        public void ShouldProvideHistoryOfOperationsOnTheWallet()
        {
            //given
            const string someOtherSourceName = "someOtherSource";
            _wallet.Add(TestSourceName, new Moneyz(5));
            _wallet.Subtract(TestSourceName, new Moneyz(4));
            _wallet.Transfer(TestSourceName, someOtherSourceName, new Moneyz(1));

            //when
            var fullHistory = _wallet.GetFullHistory();

            //then
            Assert.That(fullHistory.Operations.Count, Is.EqualTo(3));
            Assert.IsTrue(fullHistory.Operations.All(operation => operation.Source == TestSourceName));
            Assert.That(fullHistory.Operations[0].Type, Is.EqualTo(OperationType.In));
            Assert.That(fullHistory.Operations[0].HowMuch, Is.EqualTo(new Moneyz(5)));
            Assert.That(fullHistory.Operations[1].Type, Is.EqualTo(OperationType.Out));
            Assert.That(fullHistory.Operations[1].HowMuch, Is.EqualTo(new Moneyz(4)));
            Assert.That(fullHistory.Operations[2].Type, Is.EqualTo(OperationType.Transfer));
            Assert.That(fullHistory.Operations[2].HowMuch, Is.EqualTo(new Moneyz(1)));
            Assert.That(fullHistory.Operations[2].Destination, Is.EqualTo(someOtherSourceName));
        }

        [Test]
        public void ShouldProvideHistoryForTheMonth()
        {
            //given
            var today = DateTime.Today;
            var yesterMonth = today.Subtract(TimeSpan.FromDays(32));
            _timeMasterMock.SetupGet(mock => mock.Today).Returns(yesterMonth);
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(yesterMonth);
            _wallet.Add(TestSourceName, new Moneyz(4));
            _timeMasterMock.SetupGet(mock => mock.Today).Returns(today);
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(today);
            _wallet.Add(TestSourceName, new Moneyz(5));

            //when
            var historyForThisMonth = _wallet.GetHistoryForThisMonth();

            //then
            Assert.That(historyForThisMonth.Operations.Count, Is.EqualTo(1));
            Assert.That(historyForThisMonth.Operations.First().HowMuch, Is.EqualTo(new Moneyz(5)));
        }
    }
}
