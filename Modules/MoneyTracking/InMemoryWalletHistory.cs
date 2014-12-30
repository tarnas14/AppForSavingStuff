namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.NetworkInformation;

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

        public IEnumerable<Source> GetSources()
        {
            var changes = new List<Change>();
            _operations.ForEach(operation => changes.AddRange(operation.Changes));
            var sourcesInOperations = changes.Select(change => change.Source).Distinct();

            foreach (var sourceName in sourcesInOperations)
            {
                var lastChange = changes.Last(change => change.Source == sourceName);

                yield return new Source(sourceName, lastChange.After);
            }
        }
    }
}