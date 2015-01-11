namespace Application
{
    using Modules;

    internal class SystemConsole : Console
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }
    }
}