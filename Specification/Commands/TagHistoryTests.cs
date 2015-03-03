namespace Specification.Commands
{
    using Halp;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Presentation;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class TagHistoryTests
    {
        private Mock<WalletHistory> _walletHistoryMock;
        private ConsoleMock _consoleMock;

        [SetUp]
        public void Setup()
        {
            _walletHistoryMock = new Mock<WalletHistory>();
            _consoleMock = new ConsoleMock();
        }

        [Test]
        public void ShouldDisplayAllTags()
        {
            //given
            _walletHistoryMock.Setup(mock => mock.GetAllTags()).Returns(new[]
            {
                new Tag("tag1"),
                new Tag("tag2"),
                new Tag("tag3")
            });

            var expectedOutput = new[]
            {
                "#tag1, #tag2, #tag3"
            };

            var command = new DisplayTagsCommand();
            var commandHandler = new DisplayTagsCommandHandler(_walletHistoryMock.Object, new WalletUi(_consoleMock));

            //when
            commandHandler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }
    }

    public class DisplayTagsCommandHandler : CommandHandler<DisplayTagsCommand>
    {
        private readonly WalletHistory _walletHistory;
        private readonly WalletUi _walletUi;

        public DisplayTagsCommandHandler(WalletHistory walletHistory, WalletUi walletUi)
        {
            _walletHistory = walletHistory;
            _walletUi = walletUi;
        }

        public void Execute(DisplayTagsCommand command)
        {
            _walletUi.DisplayTags(_walletHistory.GetAllTags());
        }
    }

    public class DisplayTagsCommand : Command
    {
    }
}
