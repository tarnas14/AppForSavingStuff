namespace Specification
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    class UserCommandFactorySpec
    {
        private CleverFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new CleverFactory();
        }

        [Test]
        public void ShouldCreateUserCommand()
        {
            //given
            const string userInput = "/testName";

            //when
            var userCommand = _factory.CreateUserCommand(userInput);

            //then
            Assert.That(userCommand.Name, Is.EqualTo("testName"));
        }

        [Test]
        public void ShouldCreateUserCommandWithParams()
        {
            //given
            const string userInput = "/testName param1 param2 param3";

            //when
            var userCommand = _factory.CreateUserCommand(userInput);

            //then
            Assert.That(userCommand.Name, Is.EqualTo("testName"));
            Assert.That(new []{"param1", "param2", "param3"}, Is.EquivalentTo(userCommand.Params));
        }
    }

    internal class CleverFactory : UserCommandFactory
    {
        public UserCommand CreateUserCommand(string userInput)
        {
            var withoutLeadingSlash = userInput.TrimEnd().Substring(1);
            var brokenDown = withoutLeadingSlash.Split(' ');
            var name = brokenDown.First();
            var commandParams = brokenDown.Skip(1).ToList();

            return new UserCommand
            {
                Name = name,
                Params = commandParams
            };
        }
    }
}
