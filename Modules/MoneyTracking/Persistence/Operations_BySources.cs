namespace Modules.MoneyTracking.Persistence
{
    using System.Linq;
    using Raven.Client.Indexes;

    public class Operations_BySources : AbstractIndexCreationTask<Operation>
    {
        public override bool IsMapReduce
        {
            get { return false; }
        }

        public class Result
        {
            public string SourceName { get; set; }
        }

        public Operations_BySources()
        {
            Map = operations => from operation in operations
                from change in operation.Changes
                select new Result
                {
                    SourceName = change.Source
                };
        }
    }
}