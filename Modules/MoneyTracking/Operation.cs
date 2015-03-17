﻿namespace Modules.MoneyTracking
{
    using System;
    using System.Collections.Generic;

    public class Operation
    {
        public string Id { get; set; }

        public DateTime When { get; private set; }
        public IList<Change> Changes { get; private set; }
        public string Description { get; set; }
        public IList<Tag> Tags { get; set; }

        public Operation()
        {
            
        }

        public Operation(DateTime when)
        {
            When = when;
            Changes = new List<Change>();
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