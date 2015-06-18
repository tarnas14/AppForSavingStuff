namespace Modules.MoneyTracking
{
    using System;
    using System.Collections.Generic;

    public class Operation
    {
        public string Id { get; set; }

        public DateTime When { get; private set; }
        public IList<Change> Changes { get; set; }
        public string Description { get; set; }
        public IList<Tag> Tags { get; set; }
        public IList<string> TagStrings { get; set; }

        public Operation()
        {
            Changes = new List<Change>();
            TagStrings = new List<string>();
        }

        public Operation(DateTime when) : this()
        {
            When = when;
        }

        public Operation AddChange(string sourceName, Moneyz howMuch)
        {
            Changes.Add(new Change
            {
                Source = sourceName,
                Difference = howMuch
            });

            return this;
        }
    }
}