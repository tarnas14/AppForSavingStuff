namespace Specification.Presentation
{
    using System.Collections.Generic;
    using Halp;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;
    using Tarnas.ConsoleUi;

    [TestFixture]
    class WalletUiTests
    {
        private WalletUi _walletUi;
        private ConsoleMock _consoleMock;

        [SetUp]
        public void Setup()
        {
            _consoleMock = new ConsoleMock();
            _walletUi = new WalletUi(_consoleMock)
            {
                TabSize = 4
            };
        }

        [Test]
        public void ShouldDisplaySingleBalance()
        {
            //given
            var balances = new Balances();
            var balance = new Moneyz(4);
            const string balanceDisplay = "someBalance";
            balances.AddBalance(balanceDisplay, balance);
            var expectedLines = new List<string>
            {
                "    someBalance: 4.00"
            };

            //when
            _walletUi.DisplayBalance(balances);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        [Test]
        public void ShouldDisplayMultipleBalances()
        {
            //given
            var balances = new Balances();
            balances.AddBalance("someBalance", new Moneyz(14));
            balances.AddBalance("someOtherLongerBalance", new Moneyz(4));
            var expectedLines = new List<string>
            {
                "               someBalance: 14.00",
                "    someOtherLongerBalance:  4.00",
                "                          : 18.00"
            };

            //when
            _walletUi.DisplayBalance(balances);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }
    }
}
