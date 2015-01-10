namespace Specification.Halp
{
    using System.Collections.Generic;
    using Modules;

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
