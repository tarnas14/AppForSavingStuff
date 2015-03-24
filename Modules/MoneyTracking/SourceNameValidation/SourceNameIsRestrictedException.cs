namespace Modules.MoneyTracking.SourceNameValidation
{
    public class SourceNameIsRestrictedException : WalletException
    {
        public SourceNameIsRestrictedException()
            : base("'tags' is a reserved word and cannot be used as a source name.")
        {
        }
    }
}
