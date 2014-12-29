namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public interface WalletHistory
    {
        void SaveOperation(Operation toSave);
        IList<Operation> GetAll();
        IList<Operation> GetForMonth(int year, int month);
    }
}