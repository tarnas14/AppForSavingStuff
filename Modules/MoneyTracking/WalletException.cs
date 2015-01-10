namespace Modules.MoneyTracking
{
    using System;

    public class WalletException : Exception
    {
        public WalletException(string message) : base(message)
        {
        }
    }
}