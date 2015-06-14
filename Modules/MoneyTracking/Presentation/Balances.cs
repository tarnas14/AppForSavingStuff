namespace Modules.MoneyTracking.Presentation
{
    using System.Collections.Generic;
    using System.Linq;

    public class Balances
    {
        private readonly IDictionary<string, Moneyz> _balances;

        public Balances()
        {
            _balances = new Dictionary<string, Moneyz>();
            DisplaySum = true;
        }

        public void AddBalance(string displayName, Moneyz balance)
        {
            _balances.Add(displayName, balance);
        }

        public IDictionary<string, Moneyz> GetBalances()
        {
            return _balances;
        }

        public Moneyz TotalBalance
        {
            get { return _balances.Aggregate(new Moneyz(0), (m1, m2) => m1 + m2.Value); }
        }

        public bool DisplaySum { get; set; }

        public void AddBalance(Source source)
        {
            AddBalance(source.Name, source.Balance);
        }
    }
}