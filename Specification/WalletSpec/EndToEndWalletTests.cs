namespace Specification.WalletSpec
{
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
        [Test]
        [TestCaseSource(typeof(TestCasesDataSource), "BasicOperations")]
        public void ShouldCorrectlyControlUserInputToBasicOperations(string source, IEnumerable<string> userCommands, Moneyz expectedBalance)
        {
            //given
            var ui = new ConsoleUi(new CleverFactory());
            var walletUiMock = new Mock<WalletUi>();
            var ravenHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider() {RunInMemory = true})
            {
                WaitForNonStale = true
            };
            var walletMainController = new WalletMainController(walletUiMock.Object, new Wallet(ravenHistory, new SystemClockTimeMaster()));
            ui.Subscribe(walletMainController, "wallet");

            //when
            ExecuteCommands(ui, userCommands);
            ui.UserInput(string.Format("/wallet balance {0}", source));

            //then
            walletUiMock.Verify(mock => mock.DisplayBalance(source, expectedBalance), Times.Once);
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
                yield return new TestCaseData("mbank", new List<string> { "/wallet add mbank 2" }, new Moneyz(2)).SetName("add 2");
                yield return new TestCaseData("mbank", new List<string> { "/wallet add mbank 2 'my description' tag1 tag2" }, new Moneyz(2)).SetName("add 2 with description and tags");
                yield return new TestCaseData("mbank", new List<string>
                {
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet sub mbank 2 'my description' tag1 tag2"
                }, new Moneyz(3)).SetName("add 5 subtract 2");
                yield return new TestCaseData("mbank", new List<string>
                {
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet trans mbank getin 3 'my description' tag1 tag2"
                }, new Moneyz(2)).SetName("add 5 to mbank transfer 3 to getin display mbank");
                yield return new TestCaseData("getin", new List<string>
                {
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet trans mbank getin 3 'my description' tag1 tag2"
                }, new Moneyz(3)).SetName("add 5 to mbank transfer 3 to getin display getin");
            }
        }
    }
}
