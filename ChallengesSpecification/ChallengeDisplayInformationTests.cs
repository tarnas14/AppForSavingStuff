namespace ChallengesSpecification
{
    using System.Collections.Generic;
    using Modules.Challenges.UI;
    using NUnit.Framework;

    class ChallengeDisplayInformationTests
    {
        private ChallengingDayDisplayInformationFactory _challengingDayDisplay;
        private const char EmptySquare = '\u25A1';

        [SetUp]
        public void Setup()
        {
            _challengingDayDisplay = new ChallengingDayDisplayInformationFactory();
        }

        [Test]
        public void ShouldReturnEmptySquareForDaysWithoutChallenges()
        {
            //given
            var day = new ChallengingDay
            {
                Challenges = new List<Challenge>()
            };

            //when
            var displayInformation = _challengingDayDisplay.PrepareDisplayInformation(day);

            //then
            Assert.That(displayInformation.Character, Is.EqualTo(EmptySquare));
        }
    }
}
