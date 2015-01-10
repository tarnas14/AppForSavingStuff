namespace Specification.WalletSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private Mock<WalletUi> _walletUiMock;
        private Mock<TimeMaster> _timeMasterMock;

        [SetUp]
        public void Setup()
        {
            _ui = new ConsoleUi(new CleverFactory());
            _walletUiMock = new Mock<WalletUi>();
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(() => DateTime.Now);
            _timeMasterMock.SetupGet(mock => mock.Today).Returns(() => DateTime.Today);
            var ravenHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider() { RunInMemory = true })
            {
                WaitForNonStale = true
            };
            var walletMainController = new WalletMainController(_walletUiMock.Object, new Wallet(ravenHistory, _timeMasterMock.Object));
            _ui.Subscribe(walletMainController, "wallet");
        }

        [Test]
        [TestCaseSource(typeof(TestCasesDataSource), "BasicOperations")]
        public void ShouldCorrectlyControlUserInputToBasicOperations(string source, IEnumerable<string> userCommands, Moneyz expectedBalance)
        {
            //given
            ExecuteCommands(_ui, userCommands);

            //when
            _ui.UserInput(string.Format("/wallet balance {0}", source));

            //then
            _walletUiMock.Verify(mock => mock.DisplayBalance(source, expectedBalance), Times.Once);
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
            var expectedBalance = new Moneyz(2);

            //when
            _ui.UserInput(string.Format("/wallet month balance {0}", source));

            //then
            _walletUiMock.Verify(mock => mock.DisplayBalance(source, expectedBalance), Times.Once);
        }

        [Test]
        public void ShouldCreateSource()
        {
            //given
            const string sourceName = "test";

            //when
            _ui.UserInput(string.Format("/wallet source {0}", sourceName));
            _ui.UserInput(string.Format("/wallet balance {0}", sourceName));

            //then
            _walletUiMock.Verify(mock => mock.DisplayBalance(sourceName, new Moneyz(0)), Times.Once);
        }

        [Test]
        public void ShouldNotifyUserWhenSourceDoesNotExist()
        {
            //given
            const string sourceName = "testSource";

            //when
            _ui.UserInput(string.Format("/wallet balance {0}", sourceName));

            //then
            _walletUiMock.Verify(mock => mock.DisplayError(It.Is<SourceDoesNotExistException>(e => e.Message == string.Format("Source {0} does not exist.", sourceName))));
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
                yield return new TestCaseData("mbank", new List<string> { "/wallet source mbank", "/wallet add mbank 2" }, new Moneyz(2)).SetName("add 2");
                yield return new TestCaseData("mbank", new List<string> { "/wallet source mbank", "/wallet add mbank 2 'my description' tag1 tag2" }, new Moneyz(2)).SetName("add 2 with description and tags");
                yield return new TestCaseData("mbank", new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet sub mbank 2 'my description' tag1 tag2"
                }, new Moneyz(3)).SetName("add 5 subtract 2");
                yield return new TestCaseData("mbank", new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source getin",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet trans mbank getin 3 'my description' tag1 tag2"
                }, new Moneyz(2)).SetName("add 5 to mbank transfer 3 to getin display mbank");
                yield return new TestCaseData("getin", new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source getin",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet trans mbank getin 3 'my description' tag1 tag2"
                }, new Moneyz(3)).SetName("add 5 to mbank transfer 3 to getin display getin");
            }
        }
    }
}
