namespace Specification.Commands
{
    using Halp;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.CommandHandlers;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Modules.MoneyTracking.SourceNameValidation;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class TagHistoryTests
    {
        private ConsoleMock _consoleMock;
        private BagOfRavenMagic _standardBagOfRavenMagic;

        [SetUp]
        public void Setup()
        {
            var documentStoreProvider = new DocumentStoreProvider() {RunInMemory = true};
            _standardBagOfRavenMagic = new StandardBagOfRavenMagic(documentStoreProvider) {WaitForNonStale = true};
            _consoleMock = new ConsoleMock();
        }

        [Test]
        public void ShouldDisplayAllTags()
        {
            //given
            SaveOperation(new OperationCommand { Tags = new[] {new Tag("#tag1"), new Tag("#tag2")}});
            SaveOperation(new OperationCommand { Tags = new[] { new Tag("#tag3") } });

            var expectedOutput = new[]
            {
                "#tag1, #tag2, #tag3"
            };

            var command = new DisplayTagsCommand();
            var commandHandler = new DisplayTagsCommandHandler(_standardBagOfRavenMagic, new WalletUi(_consoleMock));

            //when
            commandHandler.Handle(command);

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        private void SaveOperation(OperationCommand operationCommand)
        {
            new OperationCommandHandler(Mock.Of<SourceNameValidator>(), _standardBagOfRavenMagic).Handle(operationCommand);
        }


    }
}
