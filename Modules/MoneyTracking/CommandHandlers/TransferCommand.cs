namespace Modules.MoneyTracking.CommandHandlers
{
    public class TransferCommand : Command
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public OperationInput OperationInput { get; set; }
    }
}