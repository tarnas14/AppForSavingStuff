namespace Modules.MoneyTracking
{
    using System.Collections.Generic;

    public class WalletHistory
    {
        public IList<Operation> Operations { get; private set; }

        public WalletHistory()
        {
            Operations = new List<Operation>();
        }

        public void SaveOperation(Operation toSave)
        {
            Operations.Add(toSave);
        }
    }
}