namespace Specification.WalletSpec.EndToEnd
{
    using System.Collections.Generic;
    using NUnit.Framework;

    class DisplayBalanceTests : EndToEndBaseFixture
    {
        [Test]
        [TestCaseSource(typeof(TestCasesDataSource), "DisplayingBalance")]
        public void ShouldCorrectlyRespondToUserInput(IEnumerable<string> userCommands, IList<string> expectedOutput)
        {
            EndToEndTest(userCommands, expectedOutput);
        }

        private static class TestCasesDataSource
        {
            public static IEnumerable<TestCaseData> DisplayingBalance
            {
                get
                {
                    yield return new TestCaseData(new List<string>
                    {
                        "/wallet source mbank",
                        "/wallet source otherSource",
                        "/wallet add mbank 2",
                        "/wallet add otherSource 10",
                        "/wallet balance"
                    }, new List<string>{
                        "          mbank:  2.00",
                        "    otherSource: 10.00",
                        "               : 12.00"
                    }).SetName("should display balances of all sources if no source specified");
                }
            }
        }
    }
}