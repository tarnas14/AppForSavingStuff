namespace Specification.Commands
{
    using System;
    using Modules;
    using Modules.MoneyTracking;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class DisplayBalanceTests
    {
        private Mock<TimeMaster> _timeMasterMock;
        private Mock<WalletHistory> _walletHistoryMock;
        private Mock<ReservedWordsStore> _reservedWordsStoreMock;

        [SetUp]
        public void Setup()
        {
            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(timeMaster => timeMaster.Now).Returns(DateTime.Now);

            _walletHistoryMock = new Mock<WalletHistory>();
            _walletHistoryMock.Setup(walletHistory => walletHistory.GetBalance(It.IsAny<string>()))
                .Returns(new Moneyz(2));

            _reservedWordsStoreMock = new Mock<ReservedWordsStore>();
            _reservedWordsStoreMock.Setup(mock => mock.IsReserved(It.IsAny<string>())).Returns(false);
        }

    }
}
