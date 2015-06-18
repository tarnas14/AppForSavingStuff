namespace Modules.MoneyTracking.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Raven.Abstractions.Extensions;
    using Tarnas.ConsoleUi;
    using Console = Tarnas.ConsoleUi.Console;

    public class WalletUi
    {
        private readonly Console _console;

        private string Tab
        {
            get
            {
                return new string(' ', TabSize);
            }
        }

        public WalletUi(Console console)
        {
            _console = console;
            TabSize = 4;
        }

        public int TabSize { get; set; }

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
            var tableDisplay = new TableDisplay(_console)
            {
                SeparateHeader = true
            };
            tableDisplay.AddColumns(RequiredDataToColumns(history.Operations));

            if (verbosity.Descriptions)
            {
                tableDisplay.AddColumn(GetColumn("description", history.Operations, GetDescription));
            }

            if (verbosity.Tags)
            {
                tableDisplay.AddColumn(GetColumn("tags", history.Operations, GetTagString));
            }

            tableDisplay.Display();
        }

        private Column GetColumn(string header, IEnumerable<Operation> operations, Func<Operation, string> stringFromOperation)
        {
            var column = new Column
            {
                Header = header,
                Prefix = "  "
            };

            foreach (var operation in operations)
            {
                column.Data.Add(stringFromOperation(operation));

                if (operation.Changes.Count == 2)
                {
                    column.Data.Add(string.Empty);
                    column.Data.Add(string.Empty);
                }
            }

            return column;
        }

        private string GetDescription(Operation operation)
        {
            string description = operation.Description.Length > 28 ? string.Format("{0}..", operation.Description.Substring(0, 28)) : operation.Description;
            return string.Format("'{0}'", description);
        }

        private string GetTagString(Operation operation)
        {
            if (!operation.TagStrings.Any())
            {
                return string.Empty;
            }

            var sBuilder = new StringBuilder();

            for (int i = 0; i < operation.TagStrings.Count; ++i)
            {
                sBuilder.Append(operation.TagStrings[i]);
                if (operation.TagStrings.Count - 1 != i)
                {
                    sBuilder.Append(" ");
                }
            }
            return sBuilder.ToString();
        }

        private IEnumerable<Column> RequiredDataToColumns(IEnumerable<Operation> operations)
        {
            var columns = new List<Column>
            {
                new Column
                {
                    Header = "when",
                    Prefix = "    ",
                    Suffix = "  "
                },
                new Column
                {
                    Header = "where",
                    Suffix = "  "
                },
                new Column
                {
                    Header = "howMuch",
                    Suffix = "  ",
                    AlignRight = true
                },
                new Column
                {
                    Header = "valueAfter",
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

        private string UnsignedValueChange(Change change)
        {
            return (change.Difference).UnsignedString;
        }

        private string SignedValueChange(Change change)
        {
            return (change.Difference).SignedString;
        }

        public void DisplayTags(IEnumerable<Tag> tags)
        {
            var ordered = tags.Select(tag => Tag.IsTagName(tag.Value) ? tag.Value : "#" + tag.Value).OrderBy(t => t, StringComparer.InvariantCulture);

            _console.WriteLine(string.Join(", ", ordered.ToList()));
        }

        public void DisplayBalance(Balances balancesToDisplay)
        {
            var displayNameColumn = new Column
            {
                Prefix = Tab,
                Suffix = ": ",
                AlignRight = true
            };
            var valuesColumn = new Column
            {
                AlignRight = true
            };

            var balancesDictionary = balancesToDisplay.GetBalances();
            balancesDictionary.ForEach(pair =>
            {
                displayNameColumn.Data.Add(pair.Key);
                valuesColumn.Data.Add(pair.Value.ToString());
            });

            if (balancesDictionary.Count > 1)
            {
                if (balancesToDisplay.DisplaySum)
                {
                    valuesColumn.Data.Add(balancesToDisplay.TotalBalance.ToString());
                }
                else
                {
                    valuesColumn.Data.Add("[-]");
                }
            }

            var table = new TableDisplay(_console);
            table.AddColumn(displayNameColumn);
            table.AddColumn(valuesColumn);

            table.DisplayHeaderless();
        }

        public void DisplayInformation(string information)
        {
            _console.WriteLine(Tab + information);
        }
    }
}