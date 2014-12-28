namespace Specification
{
    using ConsoleUi;
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
}
