namespace Specification.WalletSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Modules;
    using Modules.MoneyTracking;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class WalletSpec
    {
        protected const string TestSourceName = "testSource";
        protected Wallet Wallet;
        protected Mock<TimeMaster> TimeMasterMock;

        [SetUp]
        public void Setup()
        {
            TimeMasterMock = new Mock<TimeMaster>();
            Wallet = new Wallet(new InMemoryWalletHistory(), TimeMasterMock.Object);
        }

        [Test]
        public void ShouldAddResourcesToSource()
        {
            //given
            var howMuch = new Moneyz(6.66m);

            //when
            Wallet.Add(TestSourceName, new OperationInput
            {
                HowMuch = howMuch
            });
            var currentBalance = Wallet.GetBalance(TestSourceName);

            //then
            Assert.That(currentBalance, Is.EqualTo(howMuch));
        }

        [Test]
        public void ShouldSubtractResourcesFromSource()
        {
            //given
            var howMuch = new Moneyz(6.66m);

            //when
            Wallet.Subtract(TestSourceName, new OperationInput{ HowMuch = howMuch });
            var currentBalance = Wallet.GetBalance(TestSourceName);

            //then
            Assert.That(currentBalance, Is.EqualTo(new Moneyz(-6.66m)));
        }

        [Test]
        public void ShouldTransferResourcesFromOneSourceToTheOther()
        {
            //given
            Wallet.Add("source", new OperationInput{ HowMuch = new Moneyz(5) });
            Wallet.Add("destination", new OperationInput { HowMuch = new Moneyz(2) });

            //when
            Wallet.Transfer("source", "destination", new OperationInput { HowMuch = new Moneyz(2) });
            var sourceBalance = Wallet.GetBalance("source");
            var destinationBalance = Wallet.GetBalance("destination");

            //then
            Assert.That(sourceBalance, Is.EqualTo(new Moneyz(3)));
            Assert.That(destinationBalance, Is.EqualTo(new Moneyz(4)));
        }

        [Test]
        public void ShouldStoreAddOperation()
        {
            //given
            var now = new DateTime(2012, 12, 12);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            Wallet.Add(TestSourceName, new OperationInput
            {
                HowMuch = howMuch
            });

            //when
            var fullHistory = Wallet.GetFullHistory();

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
        public void ShouldStoreAdditionalMetadataOnAddOperation()
        {
            //given
            var now = new DateTime(2012, 12, 12);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            const string description = "test description";
            var tags = new List<Tag> { new Tag("random"), new Tag("food") };
            Wallet.Add(TestSourceName, new OperationInput
            {
                HowMuch = howMuch,
                Description = description,
                Tags = tags
            });

            //when
            var fullHistory = Wallet.GetFullHistory();
            var operation = fullHistory.Operations.First();

            //then
            Assert.That(operation.Description, Is.EqualTo(description));
            Assert.That(operation.Tags, Is.EqualTo(tags));
        }

        [Test]
        public void ShouldStoreSubtractOperation()
        {
            //given
            var now = new DateTime(2012, 12, 12);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            Wallet.Subtract(TestSourceName, new OperationInput{ HowMuch = howMuch });

            //when
            var fullHistory = Wallet.GetFullHistory();

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
        public void ShouldStoreAdditionalMetadataOnSubtractOperation()
        {
            //given
            var now = new DateTime(2012, 12, 12);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            const string description = "test description";
            var tags = new List<Tag> { new Tag("tag1"), new Tag("tag2") };
            Wallet.Subtract(TestSourceName, new OperationInput
            {
                HowMuch = howMuch,
                Description = description,
                Tags = tags
            });

            //when
            var operation = Wallet.GetFullHistory().Operations.First();

            //then
            Assert.That(operation.Description, Is.EqualTo(description));
            Assert.That(operation.Tags, Is.EqualTo(tags));
        }

        [Test]
        public void ShouldStoreTransferOperation()
        {
            //given
            var when = new DateTime(2013, 12, 13);
            TimeMasterMock.SetupSequence(mock => mock.Now)
                .Returns(when);
            const string destinationName = "destination";
            Wallet.Transfer(TestSourceName, destinationName, new OperationInput { HowMuch = new Moneyz(5) });

            //when
            var fullHistory = Wallet.GetFullHistory();

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
        public void ShouldStoreAdditionalMetadataOnTransferOperation()
        {
            //given
            var when = new DateTime(2013, 12, 13);
            TimeMasterMock.SetupSequence(mock => mock.Now)
                .Returns(when);
            const string destinationName = "destination";
            const string description = "test description";
            var tags = new List<Tag> { new Tag("tag1"), new Tag("tag2") };
            Wallet.Transfer(TestSourceName, destinationName, new OperationInput
            {
                HowMuch = new Moneyz(5),
                Description = description,
                Tags = tags
            });

            //when
            var operation = Wallet.GetFullHistory().Operations.First();

            //then
            Assert.That(operation.Description, Is.EqualTo(description));
            Assert.That(operation.Tags, Is.EqualTo(tags));
        }

        [Test]
        public void ShouldProvideHistoryForTheMonth()
        {
            //given
            var today = DateTime.Today;
            var yesterMonth = today.Subtract(TimeSpan.FromDays(32));
            TimeMasterMock.SetupGet(mock => mock.Today).Returns(yesterMonth);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(yesterMonth);
            Wallet.Add(TestSourceName, new OperationInput { HowMuch = new Moneyz(4) });
            Wallet.Subtract(TestSourceName, new OperationInput { HowMuch = new Moneyz(3) });
            Wallet.Transfer(TestSourceName, "otherSource", new OperationInput { HowMuch = new Moneyz(1) });
            TimeMasterMock.SetupGet(mock => mock.Today).Returns(today);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(today);
            Wallet.Add(TestSourceName, new OperationInput { HowMuch = new Moneyz(5) });

            Thread.Sleep(2000);

            //when
            var historyForThisMonth = Wallet.GetHistoryForThisMonth();

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

        [Test]
        public void ShouldReturnFullHistoryOrderedByDate()
        {
            //given
            TimeMasterMock.SetupSequence(mock => mock.Now)
                .Returns(new DateTime(2013, 12, 11))
                .Returns(new DateTime(2013, 12, 10));
            Wallet.Add(TestSourceName, new OperationInput { HowMuch = new Moneyz(3) });
            Wallet.Transfer(TestSourceName, "destination", new OperationInput { HowMuch = new Moneyz(1) });

            //when
            var fullHistory = Wallet.GetFullHistory();

            //then
            Assert.That(fullHistory.Operations[0].When, Is.EqualTo(new DateTime(2013, 12, 10)));
            Assert.That(fullHistory.Operations[1].When, Is.EqualTo(new DateTime(2013, 12, 11)));
        }

        [Test]
        public void ShouldReturnHistoryForMonthOrderedByDate()
        {
            //given
            TimeMasterMock.SetupSequence(mock => mock.Now)
                .Returns(new DateTime(2013, 10, 12))
                .Returns(new DateTime(2013, 12, 12))
                .Returns(new DateTime(2013, 12, 11));
            TimeMasterMock.SetupGet(mock => mock.Today).Returns(new DateTime(2013, 12, 14));
            Wallet.Add(TestSourceName, new OperationInput { HowMuch = new Moneyz(3) });
            Wallet.Subtract(TestSourceName, new OperationInput { HowMuch = new Moneyz(2) });
            Wallet.Transfer(TestSourceName, "destination", new OperationInput { HowMuch = new Moneyz(1) });

            //when
            var fullHistory = Wallet.GetHistoryForThisMonth();

            //then
            Assert.That(fullHistory.Operations[0].When, Is.EqualTo(new DateTime(2013, 12, 11)));
            Assert.That(fullHistory.Operations[1].When, Is.EqualTo(new DateTime(2013, 12, 12)));
        }
    }
}
