namespace Specification.Commands
{
    using System;
    using Halp;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using NUnit.Framework;

    [TestFixture]
    class TagHistoryTests
    {
        private ConsoleMock _consoleMock;
        private WalletHistory _walletHistory;

        [SetUp]
        public void Setup()
        {
            _walletHistory = new RavenDocumentStoreWalletHistory(new DocumentStoreProvider() {RunInMemory = true})
            {
                WaitForNonStale = true
            };
            _consoleMock = new ConsoleMock();
        }

        [Test]
        public void ShouldDisplayAllTags()
        {
            //given
            _walletHistory.SaveOperation(new Operation(DateTime.Now){Tags=new []{new Tag("tag1"), new Tag("tag2")  }});
            _walletHistory.SaveOperation(new Operation(DateTime.Now) { Tags = new[] { new Tag("tag3") } });

            var expectedOutput = new[]
            {
                "#tag1, #tag2, #tag3"
            };

            var command = new DisplayTagsCommand();
            var commandHandler = new DisplayTagsCommandHandler(_walletHistory, new WalletUi(_consoleMock));

            //when
            commandHandler.Execute(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }
    }
}
