namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using Persistence;

    public class Source
    {
        public string Name { get; set; }
        public Moneyz Balance { get; set; }

        public Source()
        {
            
        }

        public Source(string name) : this(name, new Moneyz(0))
        {
            Balance = new Moneyz(0);
        }

        public Source(string name, Moneyz initialBalance)
        {
            Name = name;
            Balance = initialBalance;
        }

        public void SetBalance(Moneyz newBalance)
        {
            Balance = newBalance;
        }

        public static IList<Source> FromMapReduceResults(IEnumerable<Sources_ByChangesInOperations.Result> results)
        {
            return results.Select(result => new Source
            {
                Name = result.Name,
                Balance = new Moneyz(result.Balance)
            }).ToList();
        }
    }
}