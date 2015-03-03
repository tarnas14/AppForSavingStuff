namespace Specification.WalletSpec.EndToEnd
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    class BasicOperationTests : EndToEndBaseFixture
    {
        [Test]
        [TestCaseSource(typeof(TestCasesDataSource), "BasicOperations")]
        public void ShouldCorrectlyRespondToUserInput(IEnumerable<string> userCommands, IList<string> expectedOutput)
        {
            EndToEndTest(userCommands, expectedOutput);
        }

        private static class TestCasesDataSource
        {
            public static IEnumerable<TestCaseData> BasicOperations
            {
                get
                {
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet balance mbank"
                }, new List<string>{
                    "    mbank: 0.00"
                }).SetName("creating source");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source otherSource",
                    "/wallet balance all"
                }, new List<string>{
                    "          mbank: 0.00",
                    "    otherSource: 0.00",
                    "               : 0.00"
                }).SetName("should display balances of all sources");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source tags"
                }, new List<string>
                {
                    "    Error: 'tags' is a reserved word and cannot be used as a source name."
                }).SetName("'tags' should be a reserved word");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source all"
                }, new List<string>
                {
                    "    Error: 'all' is a reserved word and cannot be used as a source name."
                }).SetName("'all' should be a reserved word");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source mbank"
                }, new List<string>
                {
                    "    Error: Source mbank already exists."
                }).SetName("creating source that alredy exists");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet balance mbank"
                }, new List<string>{
                    "    Error: Source mbank does not exist."
                }).SetName("balance - source does not exist");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank", 
                    "/wallet add mbank 2", 
                    "/wallet balance mbank"
                }, new List<string>
                {
                    "    mbank: 2.00"
                }).SetName("add 2");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank", 
                    "/wallet add mbank 2 'my description' tag1 tag2", 
                    "/wallet balance mbank"
                }, new List<string>
                {
                    "    mbank: 2.00"
                }).SetName("add 2 with description and tags");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet sub mbank 2 'my description' tag1 tag2",
                    "/wallet balance mbank"
                }, new List<string>
                {
                    "    mbank: 3.00"
                }).SetName("add 5 subtract 2");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source getin",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet trans mbank getin 3 'my description' tag1 tag2",
                    "/wallet balance mbank"
                }, new List<string>
                {
                    "    mbank: 2.00"
                }).SetName("add 5 to mbank transfer 3 to getin display mbank");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source getin",
                    "/wallet add mbank 5 'my description' tag1 tag2",
                    "/wallet trans mbank getin 3 'my description' tag1 tag2",
                    "/wallet balance getin"
                }, new List<string>
                {
                    "    getin: 3.00"
                }).SetName("add 5 to mbank transfer 3 to getin display getin");
                    yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet source getinBank",
                    "/wallet add getinBank 20",
                    "/wallet add mbank 2 biedra tag1 tag2",
                    "/wallet add mbank 10 'za pizze' tag1",
                    "/wallet sub getinBank 10 orient tag1",
                    "/wallet month --t tag1"
                }, new List<string>
                {
                    "        mbank:  12.00",
                    "    getinBank: -10.00",
                    "             :   2.00"
                }).SetName("display balance for tags");
                yield return new TestCaseData(new List<string>
                {
                    "/wallet source mbank",
                    "/wallet add mbank 20 asdf tag1",
                    "/wallet sub mbank 2 qwer tag3 tag2",
                    "/wallet tags"
                },
                    new List<string>
                {
                    "#tag1, #tag2, #tag3"
                }).SetName("should display tags");
                }
            }
        }
    }
}
