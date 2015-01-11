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
            var flags = expressions.Skip(1).Where(IsFlag).ToList();
            flags.ForEach(flag => expressions.Remove(flag));
            var commandParams = expressions.Skip(1);

            return new UserCommand
            {
                Name = name,
                Params = commandParams.ToList(),
                Flags = flags.Select(flag => flag.Substring(2)).ToList()
            };
        }

        private bool IsFlag(string arg)
        {
            return arg.StartsWith("--");
        }
    }
}