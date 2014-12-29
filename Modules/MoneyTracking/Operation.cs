namespace Modules.MoneyTracking
{
    using System.Security.Cryptography;

    public class Operation
    {
        public string Source { get; private set; }
        public OperationType Type { get; private set; }
        public Moneyz HowMuch { get; private set; }
        public string Destination { get; private set; }

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