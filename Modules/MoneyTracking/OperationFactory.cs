namespace Modules.MoneyTracking
{
    public class OperationFactory
    {
        private readonly TimeMaster _timeMaster;

        public OperationFactory(TimeMaster timeMaster)
        {
            _timeMaster = timeMaster;
        }

        public Operation GetInOperation(string sourceName, Moneyz howMuch)
        {
            return new Operation
            {
                Type = OperationType.In,
                When = _timeMaster.Now,
                Source = sourceName,
                HowMuch = howMuch
            };
        }

        public Operation GetOutOperation(string sourceName, Moneyz howMuch)
        {
            return new Operation
            {
                Type = OperationType.Out,
                When = _timeMaster.Now,
                Source = sourceName,
                HowMuch = howMuch
            };
        }

        public Operation GetTransferOperation(string sourceName, string destinationName, Moneyz howMuch)
        {
            return new Operation
            {
                Type = OperationType.Transfer,
                When = _timeMaster.Now,
                Source = sourceName,
                Destination = destinationName,
                HowMuch = howMuch
            };
        }
    }
}