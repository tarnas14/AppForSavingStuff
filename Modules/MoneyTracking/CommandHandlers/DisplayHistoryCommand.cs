namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
    using Presentation;

    public class DisplayHistoryCommand : Command
    {
        public DisplayHistoryCommand()
        {
            Sources = new string[] {};
        }

        public IList<string> Sources { get; set; }
        public bool Monthly { get; set; }
        public bool DisplayTags { get; set; }
        public bool DisplayDescriptions { get; set; }
    }
}