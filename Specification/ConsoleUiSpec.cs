namespace Specification
{
    using ConsoleUi;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class ConsoleUiSpec
    {
        private Ui _ui;
        private Mock<Subscriber> _subscriberMock;
        private Mock<Subscriber> _subscriberMock2;
        private Mock<Subscriber> _subscriberMock3;
        private Mock<UserCommandFactory> _userCommandFactoryMock;

        [SetUp]
        public void Setup()
        {
            _userCommandFactoryMock = new Mock<UserCommandFactory>();
            _ui = new Ui(_userCommandFactoryMock.Object);
            _subscriberMock = new Mock<Subscriber>();
            _subscriberMock2 = new Mock<Subscriber>();
            _subscriberMock3 = new Mock<Subscriber>();
        }

        [Test]
        public void ShouldCallAllSubsForSpecificCommand()
        {
            //given
            const string commandName = "someCommand";
            _ui.Subscribe(_subscriberMock.Object, commandName);
            _ui.Subscribe(_subscriberMock2.Object, "someOtherCommand");
            _ui.Subscribe(_subscriberMock3.Object, commandName);
            _userCommandFactoryMock.Setup(mock => mock.CreateUserCommand(It.IsAny<string>()))
                .Returns(new UserCommand {Name=commandName});

            //when
            _ui.UserInput("/someCommand");

            //then
            _subscriberMock.Verify(sub => sub.Execute(It.Is<UserCommand>(u => u.Name == commandName)), Times.Once);
            _subscriberMock2.Verify(sub => sub.Execute(It.IsAny<UserCommand>()), Times.Never);
            _subscriberMock3.Verify(sub => sub.Execute(It.Is<UserCommand>(u => u.Name == commandName)), Times.Once);
        }

        [Test]
        public void ShouldProvideSubsWithProperCommand()
        {
            //given
            const string TestUserInput = "/someCommand some params";
            var expectedCommand = new UserCommand{Name = "someCommand"};
            _userCommandFactoryMock.Setup(mock => mock.CreateUserCommand(TestUserInput)).Returns(expectedCommand);
            _ui.Subscribe(_subscriberMock.Object, "someCommand");

            //when
            _ui.UserInput(TestUserInput);

            //then
            _subscriberMock.Verify(mock => mock.Execute(expectedCommand), Times.Once);
        }
    }
}
