namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;

    public class DisplayBalanceCommand : Command
    {
        public IEnumerable<string> Sources { get; set; }
    }
}