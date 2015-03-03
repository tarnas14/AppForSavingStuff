namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public interface WalletHistory
    {
        void SaveOperation(Operation toSave);
        IList<Operation> GetFullHistory();
        IList<Operation> GetForMonth(int year, int month);
        IList<Source> GetSources();
        Moneyz GetBalance(string sourceName);
        Moneyz GetSourceBalanceForThisMonth(string sourceName, int year, int month);
        void CreateSource(string sourceName);
        IList<Operation> GetTagHistoryForThisMonth(string tagName, int year, int month);
        IList<Tag> GetTagsForMonth(int year, int month);
        IList<Tag> GetAllTags(); 
        bool Exists(string sourceName);
    }
}