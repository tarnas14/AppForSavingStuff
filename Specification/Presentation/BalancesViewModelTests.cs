namespace Specification.Presentation
{
    using System.Collections.Generic;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Presentation;
    using NUnit.Framework;

    [TestFixture]
    class BalancesViewModelTests
    {
        [Test]
        public void ShouldLoadBalanceFromSource()
        {
            //given
            const string sourceName = "source";
            var balance = new Moneyz(3);
            var source = new Source(sourceName, balance);
            var balances = new Balances();
            balances.AddBalance(source);

            var expectedBalances = new Dictionary<string, Moneyz>
            {
                {sourceName, balance}
            };

            //when
            var actualBalances = balances.GetBalances();

            //then
            Assert.That(actualBalances, Is.EquivalentTo(expectedBalances));
        }
    }
}
