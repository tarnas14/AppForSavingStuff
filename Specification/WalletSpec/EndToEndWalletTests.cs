namespace Specification.WalletSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;
    using Tarnas.ConsoleUi;

    [TestFixture]
    class EndToEndWalletTests
    {
        protected ConsoleUi Ui;
        protected Mock<TimeMaster> TimeMasterMock;
        protected ConsoleMock ConsoleMock;
        public DateTime Now { get; set; }

        [SetUp]
        public virtual void Setup()
        {
            Now = new DateTime(2015, 5, 24);
            Ui = new ConsoleUi();
            TimeMasterMock = new Mock<TimeMaster>();
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(Now);
            TimeMasterMock.SetupGet(mock => mock.Today).Returns(Now);
            var ravenHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider() { RunInMemory = true })
            {
                WaitForNonStale = true
            };
            ConsoleMock = new ConsoleMock();
            var walletMainController = new WalletMainController(new WalletUi(ConsoleMock), new Wallet(ravenHistory, TimeMasterMock.Object), ravenHistory, TimeMasterMock.Object);
            Ui.Subscribe(walletMainController, "wallet");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesDataSource), "BasicOperations")]
        public void ShouldCorrectlyRespondToUserInput(IEnumerable<string> userCommands, IList<string> expectedOutput )
        {
            //given
            
            //when
            ExecuteCommands(Ui, userCommands);

            //then
            ConsoleMock.Lines.ToList().ForEach(System.Console.WriteLine);
            Assert.That(ConsoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayHistoryForCurrentMonth()
        {
            //given
            const string source = "sourceName";
            Ui.UserInput(string.Format("/wallet source {0}", source));

            TimeMasterMock.SetupGet(mock => mock.Now).Returns(new DateTime(2000, 11, 30));
            Ui.UserInput(string.Format("/wallet add {0} 4", source));

            var today = new DateTime(2000, 12, 1);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(today);
            Ui.UserInput(string.Format("/wallet add {0} 2", source));

            TimeMasterMock.SetupGet(mock => mock.Today).Returns(today);

            //when
            Ui.UserInput(string.Format("/wallet history --m"));
            var expectedOutput = new List<string>
            {
                "    when        where       howMuch  valueAfter",
                string.Empty, 
                "    2000-12-01  sourceName    +2.00        6.00"
            };

            //then
            Assert.That(ConsoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayFullHistory()
        {
            //given
            const string source =      "sourceName";
            const string otherSource = "diffSource";
            Ui.UserInput(string.Format("/wallet source {0}", source));
            Ui.UserInput(string.Format("/wallet source {0}", otherSource));

            TimeMasterMock.SetupGet(mock => mock.Now).Returns(new DateTime(2000, 11, 30));
            Ui.UserInput(string.Format("/wallet add {0} 4", source));

            var today = new DateTime(2000, 12, 1);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(today);
            Ui.UserInput(string.Format("/wallet add {0} 2", source));
            Ui.UserInput(string.Format("/wallet sub {0} 1", source));
            Ui.UserInput(string.Format("/wallet trans {0} {1} 1", source, otherSource));

            TimeMasterMock.SetupGet(mock => mock.Today).Returns(today);

            //when
            Ui.UserInput(string.Format("/wallet history"));
            var expectedOutput = new List<string>
            {
                "    when        where                   howMuch  valueAfter",
                string.Empty,
                "    2000-11-30  sourceName                +4.00        4.00",
                "    2000-12-01  sourceName                +2.00        6.00",
                "    2000-12-01  sourceName                -1.00        5.00",
                "    2000-12-01  sourceName->diffSource     1.00            ",
                "                sourceName                -1.00        4.00",
                "                diffSource                +1.00        1.00"
            };

            //then
            Assert.That(ConsoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        protected void ExecuteCommands(ConsoleUi ui, IEnumerable<string> userCommands)
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
                    "/wallet source otherSource",
                    "/wallet balance all"
                }, new List<string>{
                    "          mbank: 0.00",
                    "    otherSource: 0.00",
                    "               : 0.00"
                }).SetName("should display balances of all sources");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source tags"
                }, new List<string>
                {
                    "    Error: 'tags' is a reserved word and cannot be used as a source name."
                }).SetName("'tags' should be a reserved word");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source all"
                }, new List<string>
                {
                    "    Error: 'all' is a reserved word and cannot be used as a source name."
                }).SetName("'all' should be a reserved word");
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
                    "/wallet add mbank 2 description tag2 tag3",
                    "/wallet source getin",
                    "/wallet trans mbank getin 1 'another description' tag3 taggg",
                    "/wallet history --t"
                }, new List<string>
                {
                    "    when        where         howMuch  valueAfter  tags       ",
                    string.Empty,
                    "    2015-05-24  mbank           +2.00        2.00  tag2, tag3 ",
                    "    2015-05-24  mbank->getin     1.00              tag3, taggg",
                    "                mbank           -1.00        1.00             ",
                    "                getin           +1.00        1.00             "
                }).SetName("display month history with tags");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source getinBank",
                    "/wallet add getinBank 20",
                    "/wallet add mbank 2 biedra tag1 tag2",
                    "/wallet add mbank 10 'za pizze' tag1",
                    "/wallet sub getinBank 10 orient tag1",
                    "/wallet month --t tag1"
                }, new List<string>
                {
                    "        mbank:  12.00",
                    "    getinBank: -10.00",
                    "             :   2.00"
                }).SetName("display balance for tags");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 2 description tag2 tag3",
                    "/wallet source getin",
                    "/wallet add getin 2",
                    "/wallet trans mbank getin 1 'another description' tag3 taggg",
                    "/wallet history getin --m"
                }, new List<string>
                {
                    "    when        where         howMuch  valueAfter",
                    string.Empty,
                    "    2015-05-24  getin           +2.00        2.00",
                    "    2015-05-24  mbank->getin     1.00            ",
                    "                mbank           -1.00        1.00",
                    "                getin           +1.00        3.00"
                }).SetName("display month history for specific source");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 20 asdf tag1",
                    "/wallet sub mbank 2 qwer tag3 tag2",
                    "/wallet tags"
                },
                new List<string>
                {
                    "#tag1, #tag2, #tag3"
                }).SetName("should display tags");
            }
        }
    }
}
