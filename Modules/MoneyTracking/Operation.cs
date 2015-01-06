namespace Modules.MoneyTracking
{
    using System;
    using System.Collections.Generic;

    public class Operation
    {
        public DateTime When { get; private set; }
        public IList<Change> Changes { get; private set; }
        public string Description { get; set; }
        public IList<Tag> Tags { get; set; }

        public Operation(DateTime when)
        {
            When = when;
            Changes = new List<Change>();
        }

        public void AddChange(string sourceName, Moneyz before, Moneyz after)
        {
            Changes.Add(new Change
            {
                Source = sourceName,
                Before = before,
                After = after
            });
        }
    }
}