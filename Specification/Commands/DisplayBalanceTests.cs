namespace Specification.Commands
{
    using System.Collections.Generic;
    using Halp;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class DisplayBalanceTests
    {
        private Mock<WalletHistory> _walletHistoryMock;
        private ConsoleMock _consoleMock;
        private DisplayBalanceCommandHandler _commandHandler;

        [SetUp]
        public void Setup()
        {   
            _consoleMock = new ConsoleMock();

            _walletHistoryMock = new Mock<WalletHistory>();
            _walletHistoryMock.Setup(walletHistory => walletHistory.GetBalance(It.IsAny<string>()))
                .Returns(new Moneyz(2));

            _commandHandler = new DisplayBalanceCommandHandler(_walletHistoryMock.Object, new WalletUi(_consoleMock));
        }

        [Test]
        public void ShouldDisplayBalanceOfASingleSource()
        {
            //given
            const string sourceName = "source";
            var command = new DisplayBalanceCommand
            {
                Sources = new[] { sourceName }
            };
            _walletHistoryMock.Setup(mock => mock.GetBalance(sourceName)).Returns(new Moneyz(2));
            var expectedOutput = new List<string>
            {
                "    source: 2.00"
            };

            //when
            _commandHandler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayBalanceOfMultipleSources()
        {
            //given
            var command = new DisplayBalanceCommand
            {
                Sources = new List<string> { "source1", "source2" }
            };
            _walletHistoryMock.Setup(mock => mock.GetBalance("source1")).Returns(new Moneyz(2));
            _walletHistoryMock.Setup(mock => mock.GetBalance("source2")).Returns(new Moneyz(12));
            var expectedOutput = new List<string>
            {
                "    source1:  2.00",
                "    source2: 12.00",
                "           : 14.00"
            };

            //when
            _commandHandler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayAllSourceBalancesWhenFirstSourceSpecifiedAsAll()
        {
            //given
            var command = new DisplayBalanceCommand
            {
                Sources = new List<string> { "all" }
            };

            _walletHistoryMock.Setup(mock => mock.GetSources()).Returns(new List<Source>
            {
                new Source("source1", new Moneyz(2)),
                new Source("source2", new Moneyz(12))
            });
            var expectedOutput = new List<string>
            {
                "    source1:  2.00",
                "    source2: 12.00",
                "           : 14.00"
            };

            //when
            _commandHandler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
            _walletHistoryMock.Verify(historyMock => historyMock.GetBalance(It.IsAny<string>()), Times.Never);
        }
    }
}
