namespace Modules.MoneyTracking.Persistence
{
    using System.Linq;
    using Raven.Client.Indexes;

    public class Sources_ByChanges : AbstractIndexCreationTask<Change, Source>
    {
        public Sources_ByChanges()
        {
            Map = changes => from change in changes
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
