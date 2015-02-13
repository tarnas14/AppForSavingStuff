namespace Modules.MoneyTracking.CommandHandlers
{
    public class SubtractCommand : Command
    {
        public string Source { get; set; }
        public OperationInput OperationInput { get; set; }
    }
}