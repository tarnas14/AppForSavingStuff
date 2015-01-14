namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Ui;
    using Console = Ui.Console;

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
            var tableDisplay = new TableDisplay(_console);
            tableDisplay.AddColumns(RequiredDataToColumns(history.Operations));

            if (verbosity.Tags)
            {
                tableDisplay.AddColumn(GetTagColumn(history.Operations));
            }

            tableDisplay.Display();
        }

        private Column GetTagColumn(IEnumerable<Operation> operations)
        {
            var column = new Column
            {
                Header = "tags",
                Prefix = "  ",
                Data = new List<string> {string.Empty}
            };

            foreach (var operation in operations)
            {
                column.Data.Add(GetTagString(operation));

                if (operation.Changes.Count == 2)
                {
                    column.Data.Add(string.Empty);
                    column.Data.Add(string.Empty);
                }
            }

            return column;
        }

        private IEnumerable<Column> RequiredDataToColumns(IEnumerable<Operation> operations)
        {
            var columns = new List<Column>
            {
                new Column
                {
                    Header = "when",
                    Prefix = "    ",
                    Suffix = "  ",
                    Data = new List<string>{string.Empty}
                },
                new Column
                {
                    Header = "where",
                    Suffix = "  ",
                    Data = new List<string>{string.Empty}
                },
                new Column
                {
                    Header = "howMuch",
                    Suffix = "  ",
                    Data = new List<string>{string.Empty},
                    AlignRight = true
                },
                new Column
                {
                    Header = "valueAfter",
                    Data = new List<string>{string.Empty},
                    AlignRight = true
                }
            };

            foreach (var operation in operations)
            {
                if (operation.Changes.Count == 1)
                {
                    var change = operation.Changes.First();
                    columns[0].Data.Add(operation.When.ToString("yyyy-MM-dd"));
                    columns[1].Data.Add(change.Source);
                    columns[2].Data.Add(SignedValueChange(change));
                    columns[3].Data.Add(change.After.ToString());
                }
                else if (operation.Changes.Count == 2)
                {
                    var from = operation.Changes[0];
                    var to = operation.Changes[1];

                    columns[0].Data.Add(operation.When.ToString("yyyy-MM-dd"));
                    columns[0].Data.Add(string.Empty);
                    columns[0].Data.Add(string.Empty);

                    columns[1].Data.Add(from.Source + "->" + to.Source);
                    columns[1].Data.Add(from.Source);
                    columns[1].Data.Add(to.Source);

                    columns[2].Data.Add(UnsignedValueChange(from));
                    columns[2].Data.Add(SignedValueChange(from));
                    columns[2].Data.Add(SignedValueChange(to));

                    columns[3].Data.Add(string.Empty);
                    columns[3].Data.Add(from.After.ToString());
                    columns[3].Data.Add(to.After.ToString());
                }
            }

            return columns;
        }

        private string GetTagString(Operation operation)
        {
            if (operation.Tags.Count == 0)
            {
                return string.Empty;
            }

            var sBuilder = new StringBuilder();

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