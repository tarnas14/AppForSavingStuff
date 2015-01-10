namespace Modules.MoneyTracking
{
    public class SourceDoesNotExistException : WalletException
    {
        public SourceDoesNotExistException(string sourceName) : base(string.Format("Source {0} does not exist.", sourceName))
        {
        }
    }
}