﻿namespace Modules.MoneyTracking
{
    using System.Collections.Generic;
    using Presentation;

    public interface WalletHistory
    {
        void SaveOperation(Operation toSave);
        IList<Operation> GetAll();
        IList<Operation> GetForMonth(int year, int month, HistoryDisplayFilter filters);
        IList<Source> GetSources();
        Moneyz GetBalance(string sourceName);
        Moneyz GetSourceBalanceForThisMonth(string sourceName, int year, int month);
        void CreateSource(string sourceName);
        IList<Operation> GetTagHistoryForThisMonth(string tagName, int year, int month);
        IList<Tag> GetTagsForMonth(int year, int month);
        bool Exists(string sourceName);
    }
}