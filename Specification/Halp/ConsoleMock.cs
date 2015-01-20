namespace Specification.Halp
{
    using System.Collections.Generic;
    using Console = Tarnas.ConsoleUi.Console;

    class ConsoleMock : Console
    {
        public IList<string> Lines { get; private set; }

        public ConsoleMock()
        {
            Lines = new List<string>();
        }

        public void WriteLine(string line)
        {
            Lines.Add(line);
        }
    }
}
