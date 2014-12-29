namespace Ui
{
    using System.Collections.Generic;
    using System.Linq;

    public class CleverFactory : UserCommandFactory
    {
        public UserCommand CreateUserCommand(string userInput)
        {
            var withoutLeadingSlash = userInput.TrimEnd().Substring(1);
            var betweenApostrophies = withoutLeadingSlash.Split('\'');
            var expressions = new List<string>();
            for (int i = 0; i < betweenApostrophies.Length; ++i)
            {
                if (i%2 == 0)
                {
                    expressions.AddRange(betweenApostrophies[i].Trim().Split(' '));
                }
                else
                {
                    expressions.Add(betweenApostrophies[i].Trim());
                }
            }

            var name = expressions.First();
            var commandParams = expressions.Skip(1).ToList();

            return new UserCommand
            {
                Name = name,
                Params = commandParams
            };
        }
    }
}