namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;

    public class OperationCommand : Command
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Description { get; set; }
        public Moneyz HowMuch { get; set; }
        public IList<Tag> Tags { get; set; }
    }
}