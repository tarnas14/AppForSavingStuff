namespace Modules.MoneyTracking.Persistence
{
    class SourceAlreadyExistsException : WalletException
    {
        public SourceAlreadyExistsException(string sourceName) : base(string.Format("Source {0} already exists.", sourceName))
        {
        }
    }
}
