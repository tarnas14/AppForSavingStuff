namespace Modules.MoneyTracking.Persistence
{
    using System.Collections.Generic;
    using System.Linq;

    public static class LegacyDataMagic
    {
        public static void AddDifferencesToChanges(IEnumerable<Change> changes)
        {
            var changesWithoutDiffs = changes.Where(change => change.Difference == null);

            changesWithoutDiffs.ToList().ForEach(change =>
            {
                change.Difference = change.After - change.Before;
                change.Before = null;
            });
        }
    }
}