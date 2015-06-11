namespace Modules.MoneyTracking.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Client.Indexes;

    public class Operations_ByMonthYear : AbstractIndexCreationTask<Operation>
    {
        public override bool IsMapReduce
        {
            get { return false; }
        }

        public override string IndexName
        {
            get { return "Wallet/OperationsByMonthYer"; }
        }

        public class Result
        {
            public string MonthYear { get; set; }
            public DateTime When { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<string> Sources { get; set; } 
        }

        public Operations_ByMonthYear()
        {
            Map = operations => from operation in operations
                select new Result
                {
                    When = operation.When,
                    MonthYear = operation.When.ToString("MMyy"),
                    TagNames = operation.Tags.Select(tag => tag.Value.StartsWith("#") ? tag.Value : "#" + tag.Value),
                    Sources = operation.Changes.Select(change => change.Source)
                };
        }
    }
}
