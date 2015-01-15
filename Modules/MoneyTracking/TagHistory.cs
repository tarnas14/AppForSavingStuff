namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class TagHistory
    {
        public Tag Tag { get; set; }
        public IEnumerable<Operation> Operations { get; set; }
    }
}