namespace Application
{
    using Ui;

    internal class SystemConsole : Console
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }
    }
}