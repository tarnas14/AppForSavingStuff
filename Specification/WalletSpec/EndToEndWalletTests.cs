namespace Specification.WalletSpec
{
    using Modules.MoneyTracking;
    using Moq;
    using NUnit.Framework;
    using Ui;

    [TestFixture]
    class EndToEndWalletTests
    {
        [Test]
        public void Should()
        {
            //given
            var ui = new ConsoleUi(new CleverFactory());
            var walletUiMock = new Mock<WalletUi>();
            var walletMainController = new WalletMainController(walletUiMock.Object);
            ui.Subscribe(walletMainController, "wallet");

            //when
            ui.UserInput("/wallet add 2 mbank 'my description' tag1 tag2");
            ui.UserInput("/wallet balance mbank");

            //then
            walletUiMock.Verify(mock => mock.DisplayBalance("mbank", 2.0m), Times.Once);
        }
    }
}
