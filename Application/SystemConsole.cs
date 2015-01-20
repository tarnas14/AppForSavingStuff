namespace Application
{
    using Tarnas.ConsoleUi;

    internal class SystemConsole : Console
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }
    }
}