namespace Specification
{
    using System.Collections.Generic;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class ConsoleUiSpec
    {
        private ConsoleUi _ui;
        private Mock<Subscriber> _subscriberMock;
        private Mock<Subscriber> _subscriberMock2;
        private Mock<Subscriber> _subscriberMock3;
        private Mock<UserCommandFactory> _userCommandFactoryMock;

        [SetUp]
        public void Setup()
        {
            _userCommandFactoryMock = new Mock<UserCommandFactory>();
            _ui = new ConsoleUi(_userCommandFactoryMock.Object);
            _subscriberMock = new Mock<Subscriber>();
            _subscriberMock2 = new Mock<Subscriber>();
            _subscriberMock3 = new Mock<Subscriber>();
        }

        [Test]
        public void ShouldCallAllSubsForSpecificCommand()
        {
            //given
            _ui.Subscribe(_subscriberMock.Object, "someCommand");
            _ui.Subscribe(_subscriberMock2.Object, "someOtherCommand");
            _ui.Subscribe(_subscriberMock3.Object, "someCommand");

            //when
            _ui.UserInput("/someCommand");

            //then
            _subscriberMock.Verify(sub => sub.Execute(It.Is<UserCommand>(u => u.Name == "someCommand")), Times.Once);
            _subscriberMock2.Verify(sub => sub.Execute(It.IsAny<UserCommand>()), Times.Never);
            _subscriberMock3.Verify(sub => sub.Execute(It.Is<UserCommand>(u => u.Name == "someCommand")), Times.Once);
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

    public interface UserCommandFactory
    {
        UserCommand CreateUserCommand(string userInput);
    }

    public class UserCommand
    {
        public string Name { get; set; }
        public IList<string> Params { get; set; }
    }

    public interface Subscriber
    {
        void Execute(UserCommand userCommand);
    }

    internal class ConsoleUi
    {
        private readonly UserCommandFactory _userCommandFactory;
        private readonly SubscriberStore _subscribers;

        public ConsoleUi(UserCommandFactory userCommandFactory)
        {
            _userCommandFactory = userCommandFactory;
            _subscribers = new SubscriberStore();
        }

        public void UserInput(string userInput)
        {
            var userCommand = _userCommandFactory.CreateUserCommand(userInput);
            var subscribers = _subscribers.GetSubsFor(userCommand.Name);

            foreach (var sub in subscribers)
            {
                sub.Execute(userCommand);
            }
        }

        public void Subscribe(Subscriber subscriber, string commandName)
        {
            _subscribers.Store(subscriber, commandName);
        }
    }
}
