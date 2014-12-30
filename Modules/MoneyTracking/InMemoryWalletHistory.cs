namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryWalletHistory : WalletHistory
    {
        private readonly IList<Operation> _operations;

        private IEnumerable<Operation> OperationsByDate
        {
            get { return _operations.OrderBy(operation => operation.When); }
        }

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
            return OperationsByDate.ToList();
        }

        public IList<Operation> GetForMonth(int year, int month)
        {
            return OperationsByDate.Where(operation => operation.When.Year == year && operation.When.Month == month).ToList();
        }

        public IEnumerable<Source> GetSources()
        {
            var changes = new List<Change>();
            OperationsByDate.ToList().ForEach(operation => changes.AddRange(operation.Changes));
            var sourcesInOperations = changes.Select(change => change.Source).Distinct();

            foreach (var sourceName in sourcesInOperations)
            {
                var lastChange = changes.Last(change => change.Source == sourceName);

                yield return new Source(sourceName, lastChange.After);
            }
        }
    }
}