namespace Specification.Commands
{
    using System;
    using System.Collections.Generic;
    using Halp;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Modules.MoneyTracking.SourceNameValidation;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class DisplayBalanceTests
    {
        private ConsoleMock _consoleMock;
        private DisplayBalanceCommandHandler _commandHandler;
        private WalletHistory _walletHistory;
        private OperationCommandHandler _operationCommandHandler;

        [SetUp]
        public void Setup()
        {   
            _consoleMock = new ConsoleMock();

            var documentStoreProvider = new DocumentStoreProvider(){RunInMemory = true};
            _walletHistory = new RavenDocumentStoreWalletHistory(documentStoreProvider){WaitForNonStale = true};

            _operationCommandHandler = new OperationCommandHandler(Mock.Of<SourceNameValidator>(), new StandardBagOfRavenMagic(documentStoreProvider) {WaitForNonStale = true});

            _commandHandler = new DisplayBalanceCommandHandler(_walletHistory, new WalletUi(_consoleMock));
        }

        [Test]
        public void ShouldDisplayBalanceOfASingleSource()
        {
            //given
            const string sourceName = "source1";
            SaveOperation(new OperationCommand { Source = sourceName, HowMuch = new Moneyz(2) });
            var command = new DisplayBalanceCommand
            {
                Sources = new[] { sourceName }
            };
            var expectedOutput = new List<string>
            {
                "    source1: 2.00"
            };

            //when
            _commandHandler.Handle(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        private void SaveOperation(OperationCommand command)
        {
            _operationCommandHandler.Handle(command);
        }
        
        [Test]
        public void ShouldDisplayBalanceOfMultipleSources()
        {
            //given
            SaveOperation(new OperationCommand { Source = "source1", HowMuch = new Moneyz(2) });
            SaveOperation(new OperationCommand { Source = "source2", HowMuch = new Moneyz(12) });
            var command = new DisplayBalanceCommand
            {
                Sources = new List<string> { "source1", "source2" }
            };
            
            var expectedOutput = new List<string>
            {
                "    source1:  2.00",
                "    source2: 12.00",
                "           : 14.00"
            };

            //when
            _commandHandler.Handle(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayAllSourceBalancesWhenNoSourcesSpecified()
        {
            //given
            SaveOperation(new OperationCommand { Source = "source1", HowMuch = new Moneyz(2) });
            SaveOperation(new OperationCommand { Source = "source2", HowMuch = new Moneyz(12) });
            var command = new DisplayBalanceCommand
            {
                Sources = new List<string>()
            };

            var expectedOutput = new List<string>
            {
                "    source1:  2.00",
                "    source2: 12.00",
                "           : 14.00"
            };

            //when
            _commandHandler.Handle(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayTagBalanceForSpecifiedMonth()
        {
            //given
            var pastMonthDate = new DateTime(2015, 03, 02);
            var thisMonthDate = new DateTime(2015, 04, 02);
            var pastMonthOperation = new OperationCommand
            {
                When = pastMonthDate,
                Source = "source1",
                HowMuch = new Moneyz(2),
                Tags = new[] {new Tag("#tag1"), new Tag("#tag2")}
            };
            var thisMonthOperation = new OperationCommand
            {
                When = thisMonthDate,
                Source = "source2",
                HowMuch = new Moneyz(12),
                Tags = new[] { new Tag("#tag1"), new Tag("#tag3") }
            };

            SaveOperation(pastMonthOperation);
            SaveOperation(thisMonthOperation);

            var command = new DisplayBalanceCommand
            {
                Sources = new []{ "#tag1" },
                Month = new Month(2015, 4)
            };

            var expectedOutput = new List<string>
            {
                "    #tag1: 12.00"
            };

            //when
            _commandHandler.Handle(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayTagBalanceForSpecifiedMonthForLegacyData()
        {
            //given
            var pastMonthDate = new DateTime(2015, 03, 02);
            var thisMonthDate = new DateTime(2015, 04, 02);
            var pastMonthOperation = new OperationCommand
            {
                When = pastMonthDate,
                Source = "source1",
                HowMuch = new Moneyz(2),
                Tags = new[] { new Tag("tag1"), new Tag("tag2") }
            };
            var thisMonthOperationWithTagsWithHashes = new OperationCommand
            {
                When = thisMonthDate,
                Source = "source2",
                HowMuch = new Moneyz(2),
                Tags = new[] { new Tag("#tag1"), new Tag("#tag3") }
            };
            var thisMonthOperationWithTagsWithoutHashes = new OperationCommand
            {
                When = thisMonthDate,
                Source = "source2",
                HowMuch = new Moneyz(10),
                Tags = new[] { new Tag("tag1"), new Tag("tag3") }
            };

            SaveOperation(pastMonthOperation);
            SaveOperation(thisMonthOperationWithTagsWithHashes);
            SaveOperation(thisMonthOperationWithTagsWithoutHashes);

            var command = new DisplayBalanceCommand
            {
                Sources = new[] { "#tag1" },
                Month = new Month(2015, 4)
            };

            var expectedOutput = new List<string>
            {
                "    #tag1: 12.00"
            };

            //when
            _commandHandler.Handle(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }
    }
}
