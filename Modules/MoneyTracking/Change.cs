﻿namespace Modules.MoneyTracking
{
    public class Change
    {
        public string Source { get; set; }
        public Moneyz Before { get; set; }
        public Moneyz After { get; set; }
    }
}