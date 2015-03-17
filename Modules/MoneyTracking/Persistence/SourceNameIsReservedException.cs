namespace Modules.MoneyTracking.Persistence
{
    public class SourceNameIsReservedException : WalletException
    {
        public SourceNameIsReservedException()
            : base("'tags' is a reserved word and cannot be used as a source name.")
        {
        }
    }
}
