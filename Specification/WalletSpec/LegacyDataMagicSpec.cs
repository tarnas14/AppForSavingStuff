namespace Specification.WalletSpec
{
    using System;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;

    [TestFixture]
    class LegacyDataMagicSpec
    {
        [Test]
        public void ShouldAddDifferencesToDataWithOnlyBeforeAndAfter()
        {
            //given
            var changes = new[]
            {
                new Change
                {
                    Before = new Moneyz(0),
                    After = new Moneyz(2)
                },
                new Change
                {
                    Before = new Moneyz(2),
                    After = new Moneyz(0)
                },
                new Change
                {
                    Before = new Moneyz(3),
                    After = new Moneyz(2),
                    Difference = new Moneyz(69)
                }
            };

            //when
            LegacyDataMagic.AddDifferencesToChanges(changes);

            //then
            Assert.That(changes[0].Difference, Is.EqualTo(new Moneyz(2)));
            Assert.That(changes[1].Difference, Is.EqualTo(new Moneyz(-2)));
            Assert.That(changes[2].Difference, Is.EqualTo(new Moneyz(69)));
        }
    }
}
