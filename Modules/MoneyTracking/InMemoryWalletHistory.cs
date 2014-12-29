namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

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
    }
}