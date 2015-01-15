namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class TagHistory
    {
        public Tag Tag { get; set; }
        public IDictionary<string, Moneyz> Operations { get; set; }
    }
}