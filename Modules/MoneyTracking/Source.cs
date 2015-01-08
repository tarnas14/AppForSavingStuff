namespace Modules.MoneyTracking
{
    public class Source
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public Moneyz Balance { get; set; }

        public Source()
        {
            
        }

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