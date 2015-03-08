namespace Specification.WalletSpec.EndToEnd
{
    using NUnit.Framework;

    [TestFixture]
    class BasicOperationTests
    {
        private EndToEndTester _e2eTester;

        [SetUp]
        public void Setup()
        {
            _e2eTester = new EndToEndTester();
        }

        [Test]
        public void ShouldCreateSource()
        {
            //given
            _e2eTester.Execute("/wallet source mbank");

            //when
            _e2eTester.Execute("/wallet balance mbank");

            //then
            _e2eTester.AssertExpectedResult("    mbank: 0.00");
        }

        [Test]
        public void ShouldNotCreateSourceCalledTags()
        {
            //when
            _e2eTester.Execute("/wallet source tags");

            //then
            _e2eTester.AssertExpectedResult("    Error: 'tags' is a reserved word and cannot be used as a source name.");
        }

        [Test]
        public void ShouldNotAllowDuplicatingSources()
        {
            //given
            _e2eTester.Execute("/wallet source mbank");

            //when
            _e2eTester.Execute("/wallet source mbank");

            //then
            _e2eTester.AssertExpectedResult("    Error: Source mbank already exists.");
        }

        [Test]
        public void ShouldNotDisplayBalanceForSourceThatDoesNotExist()
        {
            //when
            _e2eTester.Execute("/wallet balance mbank");

            //then
            _e2eTester.AssertExpectedResult("    Error: Source mbank does not exist.");
        }

        [Test]
        public void ShouldAllowAddingToSources()
        {
            //given
            _e2eTester.Execute("/wallet source mbank", 
                               "/wallet add mbank 2");

            //when
            _e2eTester.Execute("/wallet balance mbank");

            //then
            _e2eTester.AssertExpectedResult("    mbank: 2.00");
        }

        [Test]
        public void ShouldAllowStoringDescriptionAndTagsWhenAddingToSources()
        {
            //given
            _e2eTester.Execute(
                "/wallet source mbank",
                "/wallet add mbank 2 'my description' tag1 tag2");

            //when
            _e2eTester.Execute("/wallet balance mbank");

            //then
            _e2eTester.AssertExpectedResult("    mbank: 2.00");
        }

        [Test]
        public void ShouldCalculateBalanceFromOperationHistory()
        {
            //given
            _e2eTester.Execute("/wallet source mbank",
                "/wallet add mbank 5 'my description' tag1 tag2",
                "/wallet sub mbank 2 'my description' tag1 tag2");

            //when
            _e2eTester.Execute("/wallet balance mbank");

            //then
            _e2eTester.AssertExpectedResult("    mbank: 3.00");
        }

        [Test]
        public void ShouldRemoveMoneyFromSourceOnTransfer()
        {
            //given
            _e2eTester.Execute("/wallet source mbank",
                "/wallet source getin",
                "/wallet add mbank 5 'my description' tag1 tag2",
                "/wallet trans mbank getin 3 'my description' tag1 tag2");

            //when
            _e2eTester.Execute("/wallet balance mbank");

            //then
            _e2eTester.AssertExpectedResult("    mbank: 2.00");
        }

        [Test]
        public void ShouldAddMoneyToDestinationOnTransfer()
        {
            //given
            _e2eTester.Execute("/wallet source mbank",
                "/wallet source getin",
                "/wallet add mbank 5 'my description' tag1 tag2",
                "/wallet trans mbank getin 3 'my description' tag1 tag2");

            //when
            _e2eTester.Execute("/wallet balance getin");

            //then
            _e2eTester.AssertExpectedResult("    getin: 3.00");
        }

        [Test]
        public void ShouldDisplayAllTags()
        {
            //given
            _e2eTester.Execute("/wallet source mbank",
                "/wallet add mbank 20 asdf tag1",
                "/wallet sub mbank 2 qwer tag3 tag2");

            //when
            _e2eTester.Execute("/wallet tags");

            //then
            _e2eTester.AssertExpectedResult("#tag1, #tag2, #tag3");
        }
    }
}
