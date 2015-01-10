namespace Specification.WalletSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Moq;
    using NUnit.Framework;
    using Ui;

    [TestFixture]
    class EndToEndWalletTests
    {
        private ConsoleUi _ui;
        private Mock<TimeMaster> _timeMasterMock;
        private ConsoleMock _consoleMock;

        [SetUp]
        public void Setup()
        {
            _ui = new ConsoleUi(new CleverFactory());
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(() => DateTime.Now);
            _timeMasterMock.SetupGet(mock => mock.Today).Returns(() => DateTime.Today);
            var ravenHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider() { RunInMemory = true })
            {
                WaitForNonStale = true
            };
            _consoleMock = new ConsoleMock();
            var walletMainController = new WalletMainController(new WalletUi(_consoleMock), new Wallet(ravenHistory, _timeMasterMock.Object));
            _ui.Subscribe(walletMainController, "wallet");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesDataSource), "BasicOperations")]
        public void ShouldCorrectlyRespondToUserInput(IEnumerable<string> userCommands, IList<string> expectedOutput )
        {
            //given
            
            //when
            ExecuteCommands(_ui, userCommands);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayBalanceForCurrentMonth()
        {
            //given
            const string source = "sourceName";
            _ui.UserInput(string.Format("/wallet source {0}", source));

            _timeMasterMock.SetupGet(mock => mock.Now).Returns(new DateTime(2000, 11, 30));
            _ui.UserInput(string.Format("/wallet add {0} 4", source));

            var today = new DateTime(2000, 12, 1);
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(today);
            _ui.UserInput(string.Format("/wallet add {0} 2", source));

            _timeMasterMock.SetupGet(mock => mock.Today).Returns(today);

            //when
            _ui.UserInput(string.Format("/wallet month balance {0}", source));
            var expectedOutput = new List<string> {"    sourceName: 2.00"};

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        private void ExecuteCommands(ConsoleUi ui, IEnumerable<string> userCommands)
        {
            userCommands.ToList().ForEach(ui.UserInput);
        }
    }

    static class TestCasesDataSource
    {
        public static IEnumerable<TestCaseData> BasicOperations
        {
            get
            {
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet balance mbank"
                }, new List<string>{
                    "    mbank: 0.00"
                }).SetName("creating source");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet balance mbank"
                }, new List<string>{
                    "Source mbank does not exist."
                }).SetName("balance - source does not exist");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank", "/wallet add mbank 2", "/wallet balance mbank"
                }, new List<string>
                {
                    "    mbank: 2.00"
                }).SetName("add 2");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank", "/wallet add mbank 2 'my description' tag1 tag2", "/wallet balance mbank"
                }, new List<string>
                {
                    "    mbank: 2.00"
                }).SetName("add 2 with description and tags");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet sub mbank 2 'my description' tag1 tag2",
                    "/wallet balance mbank"
                }, new List<string>
                {
                    "    mbank: 3.00"
                }).SetName("add 5 subtract 2");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source getin",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet trans mbank getin 3 'my description' tag1 tag2",
                    "/wallet balance mbank"
                }, new List<string>
                {
                    "    mbank: 2.00"
                }).SetName("add 5 to mbank transfer 3 to getin display mbank");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source getin",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet trans mbank getin 3 'my description' tag1 tag2",
                    "/wallet balance getin"
                }, new List<string>
                {
                    "    getin: 3.00"
                }).SetName("add 5 to mbank transfer 3 to getin display getin");
            }
        }
    }
}
