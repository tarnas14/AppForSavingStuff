namespace Specification.WalletSpec
{
    using Modules.MoneyTracking;
    using NUnit.Framework;

    [TestFixture]
    class MoneyzSpec
    {
        [Test]
        [TestCase(5, "5.00")]
        [TestCase(2.01, "2.01")]
        [TestCase(-2.01, "-2.01")]
        public void ShouldReturnValueStringWithTwoDecimals(decimal value, string expected)
        {
            //given
            var moneyz = new Moneyz(value);

            //when
            var actual = moneyz.ToString();

            //then
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(5, "+5.00")]
        [TestCase(0, "0.00")]
        [TestCase(-2, "-2.00")]
        public void ShouldReturnSignedValueStringWithTwoDecimals(decimal value, string expected)
        {
            //given
            var moneyz = new Moneyz(value);

            //when
            var actual = moneyz.SignedString;

            //then
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(5, "5.00")]
        [TestCase(0, "0.00")]
        [TestCase(-2, "2.00")]
        public void ShouldReturnUnsignedValueStringWithTwoDecimals(decimal value, string expected)
        {
            //given
            var moneyz = new Moneyz(value);

            //when
            var actual = moneyz.UnsignedString;

            //then
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(5, "+")]
        [TestCase(0, "+")]
        [TestCase(-2, "-")]
        public void ShouldReturnSign(decimal value, string expected)
        {
            //given
            var moneyz = new Moneyz(value);

            //when
            var actual = moneyz.SignString;

            //then
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldBeCastedToDecimal()
        {
            //given
            const decimal expectedValue = 6.4m;
            var moneyz = new Moneyz(expectedValue);

            //when
            decimal actualValue = moneyz;

            //then
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ShouldBeCastableFromDecimal()
        {
            //given

            const decimal expectedValue = 6.54m;

            //when
            Moneyz moneyz = expectedValue;

            //then
            Assert.That(moneyz.Value, Is.EqualTo(expectedValue));
        }
    }
}
