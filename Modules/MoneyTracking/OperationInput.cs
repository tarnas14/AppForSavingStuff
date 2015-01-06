namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class OperationInput
    {
        public Moneyz HowMuch { get; set; }
        public string Description { get; set; }
        public IList<Tag> Tags { get; set; }
    }
}