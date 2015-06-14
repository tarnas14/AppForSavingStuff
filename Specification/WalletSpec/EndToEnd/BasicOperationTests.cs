namespace Specification.WalletSpec.EndToEnd
{
    using System;
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
            _endToEnd.Execute("/wallet balance mbank 'description' #tag2 #tag2");

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
        public void ShouldTransferMoney()
        {
            //given
            _endToEnd.SetTime(new DateTime(2015, 05, 01));
            _endToEnd.Execute(
                "/wallet add mbank 5",
                "/wallet add getin 1",
                "/wallet trans mbank getin 3");

            //when
            _endToEnd.Execute("/wallet history");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where         howMuch  valueAfter",
                string.Empty,
                "    2015-05-01  mbank           +5.00        5.00",
                "    2015-05-01  getin           +1.00        1.00",
                "    2015-05-01  mbank->getin     3.00            ",
                "                mbank           -3.00        2.00",
                "                getin           +3.00        4.00");
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

        [Test]
        public void ShouldCorrectlyStoreBalanceAfterOperationForOperationsInThePast()
        {
            //given
            _endToEnd.SetTime(new DateTime(2015, 05, 01));
            _endToEnd.Execute("/wallet add mbank 2");
            _endToEnd.SetTime(new DateTime(2015, 05, 05));
            _endToEnd.Execute("/wallet add mbank 3");
            _endToEnd.Execute("/wallet add mbank 2 -date 2015-05-03");

            //when
            _endToEnd.Execute("/wallet history");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where  howMuch  valueAfter",
                string.Empty,
                "    2015-05-01  mbank    +2.00        2.00",
                "    2015-05-03  mbank    +2.00        4.00",
                "    2015-05-05  mbank    +3.00        7.00");
        }
    }
}
