namespace Modules.MoneyTracking.Persistence
{
    using System.Linq;
    using Raven.Client.Indexes;

    class Operations_ByMonthYear : AbstractIndexCreationTask<Operation>
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
        }

        public Operations_ByMonthYear()
        {
            Map = operations => from operation in operations
                select new Result
                {
                    MonthYear = operation.When.ToString("MMyy")
                };
        }
    }
}
