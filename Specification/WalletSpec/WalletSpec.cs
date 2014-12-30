namespace Specification.WalletSpec
{
    using System;
    using System.Collections.Generic;
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
            _wallet = new Wallet(new InMemoryWalletHistory(), _timeMasterMock.Object);
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
        public void ShouldStoreAddOperation()
        {
            //given
            var now = new DateTime(2012, 12, 12);
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            _wallet.Add(TestSourceName, howMuch);

            //when
            var fullHistory = _wallet.GetFullHistory();

            //then
            Assert.That(fullHistory.Operations.Count, Is.EqualTo(1));
            var operation = fullHistory.Operations.First();
            Assert.That(operation.When, Is.EqualTo(now));
            Assert.That(operation.Changes.Count, Is.EqualTo(1));
            var change = operation.Changes.First();
            Assert.That(change.Source, Is.EqualTo(TestSourceName));
            Assert.That(change.Before, Is.EqualTo(new Moneyz(0)));
            Assert.That(change.After, Is.EqualTo(howMuch));
        }

        [Test]
        public void ShouldStoreSubtractOperation()
        {
            //given
            var now = new DateTime(2012, 12, 12);
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            _wallet.Subtract(TestSourceName, howMuch);

            //when
            var fullHistory = _wallet.GetFullHistory();

            //then
            Assert.That(fullHistory.Operations.Count, Is.EqualTo(1));
            var operation = fullHistory.Operations.First();
            Assert.That(operation.When, Is.EqualTo(now));
            var change = operation.Changes.First();
            Assert.That(change.Source, Is.EqualTo(TestSourceName));
            Assert.That(change.Before, Is.EqualTo(new Moneyz(0)));
            Assert.That(change.After, Is.EqualTo(new Moneyz(-5)));
        }

        [Test]
        public void ShouldStoreTransferOperation()
        {
            //given
            var when = new DateTime(2013, 12, 13);
            _timeMasterMock.SetupSequence(mock => mock.Now)
                .Returns(when);
            const string destinationName = "destination";
            _wallet.Transfer(TestSourceName, destinationName, new Moneyz(5));

            //when
            var fullHistory = _wallet.GetFullHistory();

            //then
            Assert.That(fullHistory.Operations.Count, Is.EqualTo(1));
            var operation = fullHistory.Operations[0];
            Assert.That(operation.When, Is.EqualTo(when));
            Assert.That(operation.Changes.Count, Is.EqualTo(2));
            var firstChange = operation.Changes[0];
            var secondChange = operation.Changes[1];
            Assert.That(firstChange.Source, Is.EqualTo(TestSourceName));
            Assert.That(firstChange.Before, Is.EqualTo(new Moneyz(0)));
            Assert.That(firstChange.After, Is.EqualTo(new Moneyz(-5)));
            Assert.That(secondChange.Source, Is.EqualTo(destinationName));
            Assert.That(secondChange.Before, Is.EqualTo(new Moneyz(0)));
            Assert.That(secondChange.After, Is.EqualTo(new Moneyz(5)));
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
            _wallet.Subtract(TestSourceName, new Moneyz(3));
            _wallet.Transfer(TestSourceName, "otherSource", new Moneyz(1));
            _timeMasterMock.SetupGet(mock => mock.Today).Returns(today);
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(today);
            _wallet.Add(TestSourceName, new Moneyz(5));

            //when
            var historyForThisMonth = _wallet.GetHistoryForThisMonth();

            //then
            Assert.That(historyForThisMonth.Operations.Count, Is.EqualTo(1));
            Assert.That(historyForThisMonth.Operations.First().When.Month, Is.EqualTo(today.Month));
        }

        [Test]
        public void ShouldLoadCurrentBalanceFromHistoryOnInit()
        {
            //given
            const string otherSourceName = "otherSourceBalance";
            var walletHistoryMock = new Mock<WalletHistory>();
            walletHistoryMock.Setup(mock => mock.GetSources()).Returns(new List<Source>()
            {
                new Source(TestSourceName, new Moneyz(8)),
                new Source(otherSourceName, new Moneyz(7))
            });
            var wallet = new Wallet(walletHistoryMock.Object, Mock.Of<TimeMaster>());

            //when
            var testSourceBalance = wallet.GetBalance(TestSourceName);
            var otherSourceBalance = wallet.GetBalance(otherSourceName);

            //then
            Assert.That(testSourceBalance, Is.EqualTo(new Moneyz(8)));
            Assert.That(otherSourceBalance, Is.EqualTo(new Moneyz(7)));
        }
    }
}
