namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using Persistence;
    using Raven.Client.Linq;

    public interface WalletHistory
    {
        IList<Operation> GetFullHistory();
        Moneyz GetBalance(string sourceName);
        IList<Tag> GetAllTags();
        void RemoveSource(string source);
        IRavenQueryable<Operations_ByMonthYear.Result> QueryOperations();
    }
}