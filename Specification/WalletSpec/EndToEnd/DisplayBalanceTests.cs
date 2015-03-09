namespace Specification.WalletSpec.EndToEnd
{
    using NUnit.Framework;

    [TestFixture]
    class DisplayBalanceTests
    {
        private EndToEndTester _endToEnd;

        [SetUp]
        public void Setup()
        {
            _endToEnd = new EndToEndTester();
        }

        [Test]
        public void ShouldDisplayBalanceOfAllSources()
        {
            //given
            _endToEnd.Execute("/wallet source mbank");
            _endToEnd.Execute("/wallet source otherSource");
            _endToEnd.Execute("/wallet add mbank 2");
            _endToEnd.Execute("/wallet add otherSource 10");
            
            //when
            _endToEnd.Execute("/wallet balance");

            //then
            _endToEnd.AssertExpectedResult(
                "          mbank:  2.00",
                "    otherSource: 10.00",
                "               : 12.00"
                );
        }

    }
}