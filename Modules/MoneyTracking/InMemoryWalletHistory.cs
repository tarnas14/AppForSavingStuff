namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryWalletHistory : WalletHistory
    {
        private readonly List<Operation> _operations;

        public InMemoryWalletHistory()
        {
            _operations = new List<Operation>();
        }

        public void SaveOperation(Operation toSave)
        {
            _operations.Add(toSave);
        }

        public IList<Operation> GetAll()
        {
            return _operations;
        }

        public IList<Operation> GetForMonth(int year, int month)
        {
            return _operations.Where(operation => operation.When.Year == year && operation.When.Month == month).ToList();
        }
    }
}