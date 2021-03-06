namespace Modules.MoneyTracking.CommandHandlers
{
    using System;
    using System.Collections.Generic;

    public class OperationCommand : Command
    {
        public OperationCommand()
        {
            HowMuch = new Moneyz(0);
            Tags = new List<Tag>();
        }

        public string Source { get; set; }
        public string Destination { get; set; }
        public string Description { get; set; }
        public Moneyz HowMuch { get; set; }
        public IList<Tag> Tags { get; set; }
        public DateTime When { get; set; }
    }
}