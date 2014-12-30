namespace Modules.MoneyTracking
{
    public class Source
    {

        public string Name { get; private set; }
        public Moneyz Balance { get; private set; }

        public Source(string name) : this(name, new Moneyz(0))
        {
            Balance = new Moneyz(0);
        }

        public Source(string name, Moneyz initialBalance)
        {
            Name = name;
            Balance = initialBalance;
        }

        public void SetBalance(Moneyz newBalance)
        {
            Balance = newBalance;
        }
    }
}