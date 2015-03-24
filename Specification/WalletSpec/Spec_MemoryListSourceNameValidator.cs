namespace Specification.WalletSpec
{
    using Modules.MoneyTracking.SourceNameValidation;
    using NUnit.Framework;

    [TestFixture]
    public class Spec_MemoryListSourceNameValidator
    {
        private MemoryListSourceNameValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new MemoryListSourceNameValidator();
        }

        [Test]
        public void ShouldNotAllowRestrictedWordsAsSourceNames()
        {
            //given
            const string restricted = "asdf";
            _validator.RestrictWord(restricted);

            //when
            TestDelegate validatingARestrictedWord = () => _validator.CheckIfValid(restricted);

            //then
            Assert.That(validatingARestrictedWord, Throws.Exception.TypeOf<SourceNameIsRestrictedException>());
        }

        [Test]
        public void ShouldNotAllowWordsStartingWithHasSign()
        {
            //given
            const string notAllowed = "#sourceName";

            //when
            TestDelegate validatingWordStartingWithHash = () => _validator.CheckIfValid(notAllowed);

            //then
            Assert.That(validatingWordStartingWithHash, Throws.Exception.TypeOf<TagsNotAllowedAsSourceNameException>());
        }
    }
}