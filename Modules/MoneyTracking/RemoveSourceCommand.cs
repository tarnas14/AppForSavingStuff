namespace Modules.MoneyTracking
{
    using CommandHandlers;

    public class RemoveSourceCommand : Command
    {
        public string Source { get; set; }
    }
}