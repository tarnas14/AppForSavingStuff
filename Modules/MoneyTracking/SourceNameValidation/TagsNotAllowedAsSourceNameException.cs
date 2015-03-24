namespace Modules.MoneyTracking.SourceNameValidation
{
    public class TagsNotAllowedAsSourceNameException : WalletException
    {
        public TagsNotAllowedAsSourceNameException()
            : base("source name cannot start with a # sign.")
        {
        }
    }
}