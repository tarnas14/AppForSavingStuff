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
            FillColumns();

            DisplayHeader();

            DisplayHeaderless();
        }

        private void FillColumns()
        {
            var maxColumnHeight = _columns.Select(column => column.Height).Aggregate(0, (height1, height2) => (height1 > height2) ? height1 : height2);

            foreach (var column in _columns)
            {
                for (int i = 0; i < maxColumnHeight - column.Height; ++i)
                {
                    column.Data.Add(string.Empty);
                }
            }
        }

        private void DisplayHeader()
        {
            var headerToDisplay = _columns.Select(column => column.GetHeader());
            _console.WriteLine(headerToDisplay.Aggregate(string.Empty, (h1, h2) => h1 + h2));

            if (SeparateHeader)
            {
                _console.WriteLine(string.Empty);
            }
        }

        public void AddColumns(IEnumerable<Column> columns)
        {
            foreach (var column in columns)
            {
                AddColumn(column);
            }
        }

        public void DisplayHeaderless()
        {
            FillColumns();

            DisplayData();
        }

        private void DisplayData()
        {
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
    }
}