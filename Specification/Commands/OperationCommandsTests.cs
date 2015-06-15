namespace Specification.Commands
{
    using System;
    using System.Diagnostics;
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
        private DocumentStoreProvider _documentStoreProvider;
        private const string TestSource = "testSource";
        private const string TestDestination = "testDestination";

        [SetUp]
        public void Setup()
        {
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(timeMaster => timeMaster.Now).Returns(DateTime.Now);

            _documentStoreProvider = new DocumentStoreProvider() {RunInMemory = true};
            _walletHistory = new RavenDocumentStoreWalletHistory(_documentStoreProvider)
            {
                WaitForNonStale = true
            };

            var bagOfRavenMagic = new StandardBagOfRavenMagic(_documentStoreProvider){ WaitForNonStale = true };

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
            _commandHandler.Handle(command);

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
            _commandHandler.Handle(command);

            //then
            var balance = GetBalance(TestSource);
            Assert.That(balance, Is.EqualTo(testHowMuch));
        }

        private Moneyz GetBalance(string testSource)
        {
            using (var session = _documentStoreProvider.Store.OpenSession())
            {
                return session.Query<Source, Sources_ByChangesInOperations>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow()).First(src => src.Name == testSource)
                    .Balance;
            }
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
            _commandHandler.Handle(command);

            //then
            var balance = GetBalance(TestSource);
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
            _commandHandler.Handle(command);

            //then
            var sourceBalance = GetBalance(TestSource);
            Assert.That(sourceBalance, Is.EqualTo(new Moneyz(-howMuch)));

            var destinationBalance = GetBalance(TestDestination);
            Assert.That(destinationBalance, Is.EqualTo(new Moneyz(howMuch)));
        }

        [Test]
        public void ShouldStoreTagsInTagStrings()
        {
            //given
            const int howMuch = 666;
            var command = new OperationCommand
            {
                Source = TestSource,
                Description = Stopwatch.GetTimestamp().ToString(),
                Destination = TestDestination,
                HowMuch = new Moneyz(howMuch),
                When = DateTime.Now,
                Tags = new []{ new Tag("#tag1"), new Tag("#tag2") }
            };

            //when
            _commandHandler.Handle(command);

            //then
            using (var session = _documentStoreProvider.Store.OpenSession())
            {
                var operationWithTagsInTagStrings =
                    session.Query<Operation>().Single(operation => operation.Description == command.Description);

                Assert.That(operationWithTagsInTagStrings.TagStrings, Is.EquivalentTo(command.Tags.Select(tag => tag.Value)));
            }
        }

        [Test]
        public void ShouldStoreTags()
        {
            //given
            const int howMuch = 666;
            var command = new OperationCommand
            {
                Source = TestSource,
                Destination = TestDestination,
                HowMuch = new Moneyz(howMuch),
                When = DateTime.Now,
                Tags = new[] { new Tag(GetUniqueHashValue()), new Tag(GetUniqueHashValue()) }
            };

            //when
            _commandHandler.Handle(command);

            //then
            using (var session = _documentStoreProvider.Store.OpenSession())
            {
                var tags = session.Query<Tag>().ToList();
                Assert.That(tags.Any(tag => tag.Value == command.Tags[0].Value));
                Assert.That(tags.Any(tag => tag.Value == command.Tags[1].Value));
            }
        }

        [Test]
        public void ShouldStoreUniqueTagsInOneOperation()
        {
            //given
            const int howMuch = 666;
            var tagValue = GetUniqueHashValue();
            var command = new OperationCommand
            {
                Source = TestSource,
                Destination = TestDestination,
                HowMuch = new Moneyz(howMuch),
                When = DateTime.Now,
                Tags = new[] { new Tag(tagValue), new Tag(tagValue) }
            };

            //when
            _commandHandler.Handle(command);

            //then
            using (var session = _documentStoreProvider.Store.OpenSession())
            {
                var tags = session.Query<Tag>().ToList();
                Assert.That(tags.Count(tag => tag.Value == tagValue) == 1);
            }
        }

        private static string GetUniqueHashValue()
        {
            return "#" + Stopwatch.GetTimestamp();
        }

        [Test]
        public void ShouldStoreTagWithTheSameValueOnlyOnce()
        {
            //given
            const int howMuch = 666;
            var tagValue = GetUniqueHashValue();
            var command = new OperationCommand
            {
                Source = TestSource,
                Destination = TestDestination,
                HowMuch = new Moneyz(howMuch),
                When = DateTime.Now,
                Tags = new[] { new Tag(tagValue) }
            };

            //when
            _commandHandler.Handle(command);
            _commandHandler.Handle(command);

            //then
            using (var session = _documentStoreProvider.Store.OpenSession())
            {
                var tags = session.Query<Tag>().ToList();
                Assert.That(tags.Count(tag => tag.Value == tagValue) == 1);
            }
        }

        [Test]
        public void ShouldSanitizeTagValuesAndPrependHashesIfNecessary()
        {
            //given
            const int howMuch = 666;
            var tagValue = Stopwatch.GetTimestamp().ToString();
            var command = new OperationCommand
            {
                Source = TestSource,
                Destination = TestDestination,
                HowMuch = new Moneyz(howMuch),
                When = DateTime.Now,
                Tags = new[] { new Tag(tagValue) }
            };

            //when
            _commandHandler.Handle(command);
            _commandHandler.Handle(command);

            //then
            using (var session = _documentStoreProvider.Store.OpenSession())
            {
                var tags = session.Query<Tag>().ToList();
                Assert.That(tags.Count(tag => tag.Value == tagValue), Is.EqualTo(0));
                Assert.That(tags.Count(tag => tag.Value == "#" + tagValue), Is.EqualTo(1));
            }
        }
    }
}
