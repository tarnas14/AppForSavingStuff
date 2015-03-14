namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public interface WalletHistory
    {
        void SaveOperation(Operation toSave);
        IList<Operation> GetFullHistory();
        IList<Operation> GetForMonth(Month month);
        IList<Source> GetSources();
        Moneyz GetBalance(string sourceName);
        Moneyz GetSourceBalanceForMonth(string sourceName, Month month);
        void CreateSource(string sourceName);
        IList<Operation> GetTagHistoryForThisMonth(string tagName, Month month);
        IList<Tag> GetTagsForMonth(Month month);
        IList<Tag> GetAllTags(); 
        bool Exists(string sourceName);
    }
}