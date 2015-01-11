namespace Modules.MoneyTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Raven.Database.Server.Controllers;
    using Console = Modules.Console;

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

        public void DisplayHistory(History history)
        {
            var historyRows = new List<string[]>
            {
                new []{"when", "where", "howMuch", "valueAfter"},
                new []{string.Empty, string.Empty, string.Empty, string.Empty}
            };

            historyRows.AddRange(GetRows(history.Operations));
            var columnWidths = FindColumnWidths(historyRows);
            var columnWidthsWithMargin = new[]
            {columnWidths[0] + 2, columnWidths[1] + 2, columnWidths[2], columnWidths[3] + 2};

            historyRows.ForEach(
                row => _console.WriteLine(string.Format(
                    "    {0," + -columnWidthsWithMargin[0] + "}{1," + -columnWidthsWithMargin[1] + "}{2," + columnWidthsWithMargin[2] + "}{3," + columnWidthsWithMargin[3] + "}",
                    row[0], row[1], row[2], row[3])));
        }

        private int[] FindColumnWidths(IEnumerable<string[]> historyRows)
        {
            return historyRows
                .Select(row => new[] {row[0].Length, row[1].Length, row[2].Length, row[3].Length})
                .Aggregate(new[] {0, 0, 0, 0}, (i0, i1) => new[]
                {
                    i0[0] > i1[0] ? i0[0] : i1[0],
                    i0[1] > i1[1] ? i0[1] : i1[1],
                    i0[2] > i1[2] ? i0[2] : i1[2],
                    i0[3] > i1[3] ? i0[3] : i1[3],
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
                        CalculateValueChange(change),
                        change.After.ToString()
                        });
                }
            }

            return rows;
        }

        private string CalculateValueChange(Change change)
        {
            var valueDiff = change.After - change.Before;

            return valueDiff.Value < 0 ? valueDiff.ToString() : "+" + valueDiff;
        }
    }
}