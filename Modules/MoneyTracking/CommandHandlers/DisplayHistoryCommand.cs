namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;

    public class DisplayHistoryCommand : Command
    {
        public DisplayHistoryCommand()
        {
            Sources = new string[] {};
        }

        public IList<string> Sources { get; set; }
        public bool Monthly { get; set; }
        public Month Month { get; set; }
        public bool DisplayTags { get; set; }
        public bool DisplayDescriptions { get; set; }
    }
}