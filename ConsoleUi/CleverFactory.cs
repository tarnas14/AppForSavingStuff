namespace ConsoleUi
{
    using System.Linq;

    public class CleverFactory : UserCommandFactory
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