namespace Specification.WalletSpec
{
    using Modules.MoneyTracking;
    using NUnit.Framework;

    [TestFixture]
    class MonthSpec
    {
        [Test]
        public void ShouldCreateMonthFromString()
        {
            //given
            const int expectedYear = 2015;
            const int expectedMonth = 4;

            //when
            var month = Month.FromString(string.Format("{0}-0{1}", expectedYear, expectedMonth));

            //then
            Assert.That(month.Year, Is.EqualTo(expectedYear));
            Assert.That(month.MonthNr, Is.EqualTo(expectedMonth));
        }
    }
}
