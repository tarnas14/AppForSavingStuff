namespace Modules.MoneyTracking
{
    public class Source
    {

        public string SourceName { get; private set; }
        public Moneyz Balance { get; private set; }

        public Source(string sourceName) : this(sourceName, new Moneyz(0))
        {
            Balance = new Moneyz(0);
        }

        public Source(string sourceName, Moneyz initialBalance)
        {
            SourceName = sourceName;
            Balance = initialBalance;
        }

        public void SetBalance(Moneyz newBalance)
        {
            Balance = newBalance;
        }
    }
}