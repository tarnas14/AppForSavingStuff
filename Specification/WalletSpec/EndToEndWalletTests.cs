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
        public DateTime Now { get; set; }

        [SetUp]
        public void Setup()
        {
            Now = new DateTime(2015, 5, 24);
            _ui = new ConsoleUi(new CleverFactory());
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(Now);
            _timeMasterMock.SetupGet(mock => mock.Today).Returns(Now);
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
            _consoleMock.Lines.ToList().ForEach(System.Console.WriteLine);
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
                    "/wallet source mbank",
                    "/wallet source mbank"
                }, new List<string>
                {
                    "    Error: Source mbank already exists."
                }).SetName("creating source that alredy exists");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet balance mbank"
                }, new List<string>{
                    "    Error: Source mbank does not exist."
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
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 2.5",
                    "/wallet sub mbank 0.4",
                    "/wallet source getin",
                    "/wallet add getin 1.9",
                    "/wallet add mbank 1000.01",
                    "/wallet history"
                }, new List<string>
                {
                    "    when        where    howMuch  valueAfter",
                    "                                            ",
                    "    2015-05-24  mbank      +2.50        2.50",
                    "    2015-05-24  mbank      -0.40        2.10",
                    "    2015-05-24  getin      +1.90        1.90",
                    "    2015-05-24  mbank  +1,000.01    1,002.11",
                }).SetName("display month history without transfers");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 2",
                    "/wallet source getin",
                    "/wallet trans mbank getin 1",
                    "/wallet history"
                }, new List<string>
                {
                    "    when        where         howMuch  valueAfter",
                    "                                                 ",
                    "    2015-05-24  mbank           +2.00        2.00",
                    "    2015-05-24  mbank->getin     1.00            ",
                    "                mbank           -1.00        1.00",
                    "                getin           +1.00        1.00"
                }).SetName("display month history with transfer");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 2 description tag2 tag3",
                    "/wallet source getin",
                    "/wallet trans mbank getin 1 'another description' tag3 taggg",
                    "/wallet history --t"
                }, new List<string>
                {
                    "    when        where         howMuch  valueAfter  -- tags       ",
                    "                                                                 ",
                    "    2015-05-24  mbank           +2.00        2.00  -- tag2, tag3 ",
                    "    2015-05-24  mbank->getin     1.00              -- tag3, taggg",
                    "                mbank           -1.00        1.00                ",
                    "                getin           +1.00        1.00                "
                }).SetName("display month history with tags");
            }
        }
    }
}
