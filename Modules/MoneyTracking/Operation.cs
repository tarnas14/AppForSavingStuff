namespace Modules.MoneyTracking
{
    using System;

    public class Operation
    {
        public string Source { get; set; }
        public OperationType Type { get; set; }
        public Moneyz HowMuch { get; set; }
        public string Destination { get; set; }
        public DateTime When { get; set; }

        public static Operation In(string sourceName, Moneyz howMuch)
        {
            return new Operation
            {
                Type = OperationType.In,
                Source = sourceName,
                HowMuch = howMuch
            };
        }

        public static Operation Out(string sourceName, Moneyz howMuch)
        {
            return new Operation
            {
                Type = OperationType.Out,
                Source = sourceName,
                HowMuch = howMuch
            };
        }

        public static Operation Transfer(string source, string destination, Moneyz howMuch)
        {
            return new Operation
            {
                Type = OperationType.Transfer,
                Source = source,
                Destination = destination,
                HowMuch = howMuch
            };
        }
    }
}