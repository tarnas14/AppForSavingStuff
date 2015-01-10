namespace Modules.MoneyTracking
{
    using System;

    public class WalletOutboundUi : WalletUi
    {
        public void DisplayBalance(string sourceName, Moneyz balance)
        {
            Console.WriteLine("\t{0}: {1}", sourceName, balance);
        }

        public void DisplayError(WalletException exception)
        {
            throw new NotImplementedException();
        }
    }
}