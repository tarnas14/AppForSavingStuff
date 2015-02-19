namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
    using Presentation;

    public class DisplayHistoryCommand : Command
    {
        public DisplayHistoryCommand()
        {
            Sources = new string[] {};
            Verbosity = new HistoryDisplayVerbosity();
        }

        public IList<string> Sources { get; set; }
        public bool Monthly { get; set; }
        public HistoryDisplayVerbosity Verbosity { get; set; }
    }
}