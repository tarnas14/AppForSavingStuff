namespace Ui
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class TableDisplay
    {
        private readonly Console _console;
        private readonly ICollection<Column> _columns;

        public TableDisplay(Console console)
        {
            _console = console;
            _columns = new Collection<Column>();
        }

        public bool SeparateHeader { get; set; }

        public void AddColumn(Column column)
        {
            _columns.Add(column);
        }

        public void Display()
        {
            var headerToDisplay = _columns.Select(column => column.GetHeader());
            _console.WriteLine(headerToDisplay.Aggregate(string.Empty, (h1, h2) => h1 + h2));

            if (SeparateHeader)
            {
                _console.WriteLine(string.Empty);
            }

            if (_columns.Count == 1)
            {
                for (int i = 0; i < _columns.First().Height; ++i)
                {
                    _console.WriteLine(_columns.First().GetDataRow(i));
                }
                return;
            }

            for (int i = 0; i < _columns.First().Height; ++i)
            {
                var currRows = _columns.Select(column => column.GetDataRow(i));
                var rowToDisplay = currRows.Aggregate((r1, r2) => r1 + r2);
                _console.WriteLine(rowToDisplay);
            }
        }

        public void AddColumns(IEnumerable<Column> columns)
        {
            foreach (var column in columns)
            {
                AddColumn(column);
            }
        }
    }
}