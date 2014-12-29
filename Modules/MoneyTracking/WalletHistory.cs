namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public interface WalletHistory
    {
        void SaveOperation(Operation toSave);
        IList<Operation> GetAll();
    }
}