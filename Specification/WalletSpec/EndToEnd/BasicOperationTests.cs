namespace Specification.WalletSpec.EndToEnd
{
    using NUnit.Framework;

    [TestFixture]
    class BasicOperationTests
    {
        private EndToEndTester _endToEnd;

        [SetUp]
        public void Setup()
        {
            _endToEnd = new EndToEndTester();
        }

        [Test]
        public void ShouldNotAcceptOperationsWithSourceCalledTags()
        {
            //when
            _endToEnd.ReserveWord("tags");
            _endToEnd.Execute("/wallet add tags 2");

            //then
            _endToEnd.AssertExpectedResult("    Error: 'tags' is a reserved word and cannot be used as a source name.");
        }

        [Test]
        public void ShouldNotAcceptOperationsWithSourcesStartingWithHashSign()
        {
            //when
            _endToEnd.ReserveWord("tags");
            _endToEnd.Execute("/wallet add #source 2");

            //then
            _endToEnd.AssertExpectedResult("    Error: source name cannot start with a # sign.");
        }

        [Test]
        public void ShouldNotDisplayBalanceForSourceThatDoesNotExist()
        {
            //when
            _endToEnd.Execute("/wallet balance mbank");

            //then
            _endToEnd.AssertExpectedResult("    Error: Source mbank does not exist.");
        }

        [Test]
        public void ShouldAllowAddingToSources()
        {
            //given
            _endToEnd.Execute("/wallet add mbank 2");

            //when
            _endToEnd.Execute("/wallet balance mbank");

            //then
            _endToEnd.AssertExpectedResult("    mbank: 2.00");
        }

        [Test]
        public void ShouldAllowStoringDescriptionAndTagsWhenAddingToSources()
        {
            //given
            _endToEnd.Execute("/wallet add mbank 2");

            //when
            _endToEnd.Execute("/wallet balance mbank");

            //then
            _endToEnd.AssertExpectedResult("    mbank: 2.00");
        }

        [Test]
        public void ShouldCalculateBalanceFromOperationHistory()
        {
            //given
            _endToEnd.Execute(
                "/wallet add mbank 5",
                "/wallet sub mbank 2");

            //when
            _endToEnd.Execute("/wallet balance mbank");

            //then
            _endToEnd.AssertExpectedResult("    mbank: 3.00");
        }

        [Test]
        public void ShouldRemoveMoneyFromSourceOnTransfer()
        {
            //given
            _endToEnd.Execute(
                "/wallet add mbank 5",
                "/wallet trans mbank getin 3");

            //when
            _endToEnd.Execute("/wallet balance mbank");

            //then
            _endToEnd.AssertExpectedResult("    mbank: 2.00");
        }

        [Test]
        public void ShouldAddMoneyToDestinationOnTransfer()
        {
            //given
            _endToEnd.Execute(
                "/wallet add mbank 5",
                "/wallet trans mbank getin 3");

            //when
            _endToEnd.Execute("/wallet balance getin");

            //then
            _endToEnd.AssertExpectedResult("    getin: 3.00");
        }

        [Test]
        public void ShouldDisplayAllTags()
        {
            //given
            _endToEnd.Execute(
                "/wallet add mbank 20 #asdf #tag1",
                "/wallet sub mbank 2 qwer #tag3 #tag2");

            //when
            _endToEnd.Execute("/wallet tags");

            //then
            _endToEnd.AssertExpectedResult("#tag1, #tag2, #tag3");
        }

        [Test]
        public void ShouldAddOperationWithSpecifiedDate()
        {
            //when
            _endToEnd.Execute("/wallet add mbank 20 -date 2015-02-02");

            //then
            _endToEnd.Execute("/wallet history");
            _endToEnd.AssertExpectedResult(
                "    when        where  howMuch  valueAfter",
                string.Empty,
                "    2015-02-02  mbank   +20.00       20.00"
                );
        }

        [Test]
        public void ShouldRemoveASource()
        {
            //given
            _endToEnd.Execute("/wallet add mbank 20");
            _endToEnd.Execute("/wallet add getin 10");
            _endToEnd.Execute("/wallet remove getin");

            //when
            _endToEnd.Execute("/wallet balance");

            //then
            _endToEnd.AssertExpectedResult(
                "    getin removed",
                "    mbank: 20.00");
        }
    }
}
