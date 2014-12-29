namespace Specification.WalletSpec
{
    using Modules.MoneyTracking;
    using NUnit.Framework;

    [TestFixture]
    class WalletSpec
    {
        private const string TestSourceName = "testSource";
        private Wallet _wallet;

        [SetUp]
        public void Setup()
        {
            _wallet = new Wallet();
        }

        [Test]
        public void ShouldAddResourcesToSource()
        {
            //given
            var howMuch = new Moneyz(6.66m);

            //when
            _wallet.Add(TestSourceName, howMuch);
            var currentBalance = _wallet.GetBalance(TestSourceName);

            //then
            Assert.That(currentBalance, Is.EqualTo(howMuch));
        }

        [Test]
        public void ShouldSubtractResourcesFromSource()
        {
            //given
            var howMuch = new Moneyz(6.66m);

            //when
            _wallet.Subtract(TestSourceName, howMuch);
            var currentBalance = _wallet.GetBalance(TestSourceName);

            //then
            Assert.That(currentBalance, Is.EqualTo(new Moneyz(-6.66m)));
        }

        [Test]
        public void ShouldTransferResourcesFromOneSourceToTheOther()
        {
            //given
            _wallet.Add("source", new Moneyz(5));
            _wallet.Add("destination", new Moneyz(2));

            //when
            _wallet.Transfer("source", "destination", new Moneyz(2));
            var sourceBalance = _wallet.GetBalance("source");
            var destinationBalance = _wallet.GetBalance("destination");

            //then
            Assert.That(sourceBalance, Is.EqualTo(new Moneyz(3)));
            Assert.That(destinationBalance, Is.EqualTo(new Moneyz(4)));
        }
    }
}
