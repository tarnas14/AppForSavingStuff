namespace Modules.MoneyTracking.CommandHandlers
{
    public class AddCommand : Command
    {
        public string Source { get; set; }
        public OperationInput OperationInput { get; set; }
    }
}