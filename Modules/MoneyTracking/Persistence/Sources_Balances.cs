namespace Modules.MoneyTracking.Persistence
{
    using Raven.Client.Indexes;
    using System.Linq;

    class Sources_Balances : AbstractIndexCreationTask<Operation, Source>
    {
        public Sources_Balances()
        {
            Map = operations => from operation in operations
                                from change in operation.Changes
                                select new Source
                                {
                                    Name = change.Source,
                                    Balance = change.Difference
                                };

            Reduce = results => from result in results
                                group result by result.Name
                                into g
                                select new Source
                                {
                                    Name = g.Key,
                                    Balance = g.Sum(x => x.Balance)
                                };
        }
    }
}
