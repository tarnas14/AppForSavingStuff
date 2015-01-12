namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Raven.Database.Linq.PrivateExtensions;
    using Console = Console;

    public class WalletUi
    {
        private readonly Console _console;
        private const string Tab = "    ";

        public WalletUi(Console console)
        {
            _console = console;
        }

        public void DisplayBalance(string sourceName, Moneyz balance)
        {
            _console.WriteLine(string.Format("{0}{1}: {2}", Tab, sourceName, balance));
        }

        public void DisplayError(WalletException exception)
        {
            _console.WriteLine(string.Format("{0}Error: {1}", Tab, exception.Message));
        }

        public void DisplayHistory(History history, HistoryDisplayVerbosity verbosity)
        {
            var historyRows = new List<string[]>
            {
                new []{"when", "where", "howMuch", "valueAfter", "  -- tags"},
                new []{string.Empty, string.Empty, string.Empty, string.Empty, string.Empty}
            };

            historyRows.AddRange(GetRows(history.Operations));
            var columnWidths = FindColumnWidths(historyRows);
            var columnWidthsWithMargin = new[]
            {columnWidths[0] + 2, columnWidths[1] + 2, columnWidths[2], columnWidths[3] + 2, columnWidths[4]};

            var sBuilder = new StringBuilder();
            sBuilder.Append("    ");
            for (int i = 0; i < ((verbosity.Tags) ? 5 : 4); ++i)
            {
                sBuilder.Append("{" + i + ",");
                if (i != 2 && i != 3)
                {
                    sBuilder.Append(-columnWidthsWithMargin[i]);
                }
                else
                {
                    sBuilder.Append(columnWidthsWithMargin[i]);
                }
                sBuilder.Append("}");
            }
            string formatString = sBuilder.ToString();

            historyRows.ForEach(
                row => _console.WriteLine(string.Format(
                    formatString, row[0], row[1], row[2], row[3], row[4])));

            //historyRows.ForEach(
            //    row => _console.WriteLine(string.Format(
            //        "    {0," + -columnWidthsWithMargin[0] + "}{1," + -columnWidthsWithMargin[1] + "}{2," + columnWidthsWithMargin[2] + "}{3," + columnWidthsWithMargin[3] + "}{4," + columnWidthsWithMargin[4] + "}",
            //        row[0], row[1], row[2], row[3], row[4])));
        }

        private int[] FindColumnWidths(IEnumerable<string[]> historyRows)
        {
            return historyRows
                .Select(row => new[] {row[0].Length, row[1].Length, row[2].Length, row[3].Length, row[4].Length})
                .Aggregate(new[] {0, 0, 0, 0, 0}, (i0, i1) => new[]
                {
                    i0[0] > i1[0] ? i0[0] : i1[0],
                    i0[1] > i1[1] ? i0[1] : i1[1],
                    i0[2] > i1[2] ? i0[2] : i1[2],
                    i0[3] > i1[3] ? i0[3] : i1[3],
                    i0[4] > i1[4] ? i0[4] : i1[4],
                });
        }

        private IEnumerable<string[]> GetRows(IEnumerable<Operation> operations)
        {
            var rows = new Collection<string[]>();

            foreach (var operation in operations)
            {
                if (operation.Changes.Count == 1)
                {
                    var change = operation.Changes.First();
                    rows.Add(new []{
                        operation.When.ToString("yyyy-MM-dd"),
                        change.Source,
                        SignedValueChange(change),
                        change.After.ToString(),
                        GetTagString(operation)
                        });
                }
                else if (operation.Changes.Count == 2)
                {
                    var from = operation.Changes[0];
                    var to = operation.Changes[1];

                    rows.Add(new []
                    {
                        operation.When.ToString("yyyy-MM-dd"),
                        from.Source + "->" + to.Source,
                        UnsignedValueChange(from),
                        string.Empty,
                        GetTagString(operation)
                    });

                    rows.Add(new[]{
                        string.Empty,
                        from.Source,
                        SignedValueChange(from),
                        from.After.ToString(),
                        string.Empty
                        });

                    rows.Add(new[]{
                        string.Empty,
                        to.Source,
                        SignedValueChange(to),
                        to.After.ToString(),
                        string.Empty
                        });
                }
            }

            return rows;
        }

        private string GetTagString(Operation operation)
        {
            if (operation.Tags.Count == 0)
            {
                return string.Empty;
            }

            var sBuilder = new StringBuilder();
            sBuilder.Append("  -- ");

            for (int i = 0; i < operation.Tags.Count; ++i)
            {
                sBuilder.Append(operation.Tags[i].Value);
                if (operation.Tags.Count - 1 != i)
                {
                    sBuilder.Append(", ");
                }
            }
            return sBuilder.ToString();
        }

        private string UnsignedValueChange(Change change)
        {
            var valueDiff = change.After - change.Before;
            if (valueDiff.Value < 0)
            {
                valueDiff = new Moneyz(-valueDiff.Value);
            }

            return valueDiff.ToString();
        }

        private string SignedValueChange(Change change)
        {
            var valueDiff = change.After - change.Before;

            return valueDiff.Value < 0 ? valueDiff.ToString() : "+" + valueDiff;
        }
    }
}