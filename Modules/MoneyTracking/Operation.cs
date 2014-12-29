namespace Modules.MoneyTracking
{
    using System;

    public class Operation
    {
        public string Source { get; set; }
        public DateTime When { get; set; }
        public Moneyz Before { get; set; }
        public Moneyz After { get; set; }
    }
}