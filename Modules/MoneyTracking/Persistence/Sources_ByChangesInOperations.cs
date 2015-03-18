namespace Modules.MoneyTracking.Persistence
{
    using System.Linq;
    using Raven.Client.Indexes;

    public class Sources_ByChangesInOperations : AbstractIndexCreationTask<Operation, Sources_ByChangesInOperations.Result>
    {
        public override bool IsMapReduce
        {
            get { return true; }
        }

        public Sources_ByChangesInOperations()
        {
            Map = operations => from operation in operations
                from change in operation.Changes
                select new
                {
                    Name = change.Source,
                    Balance = change.Difference == null ? (change.After.Value - change.Before.Value) : change.Difference.Value
                };
            Reduce = results => from result in results
                group result by result.Name
                into g
                select new
                {
                    Name = g.Key,
                    Balance = g.Sum(x => x.Balance)
                };
        }

        public class Result
        {
            public string Name { get; set; }
            public decimal Balance { get; set; }
        }
    }
}