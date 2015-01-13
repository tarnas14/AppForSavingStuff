namespace Ui
{
    using System.Collections.Generic;
    using System.Linq;

    public class Column
    {
        public Column()
        {
            Data = new List<string>();
        }

        public string Header { get; set; }
        public IList<string> Data { get; set; }

        public int Width
        {
            get { return Data.Select(d => d.Length).Aggregate(Header.Length, (l1, l2) => l1 > l2 ? l1 : l2); }
        }

        public int Height
        {
            get { return Data.Count + 1; }
        }

        public IEnumerable<string> GetRows()
        {
            throw new System.NotImplementedException();
        }

        public string GetRow(int rowId)
        {
            if (rowId < 0 || rowId >= Height)
            {
                return Format(string.Empty);
            }

            if (rowId == 0)
            {
                return Format(Header);
            }

            return Format(Data[rowId - 1]);
        }

        private string Format(string data)
        {
            return string.Format("{0,-" + Width + "}", data);
        }
    }
}