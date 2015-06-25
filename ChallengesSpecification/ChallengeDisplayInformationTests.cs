namespace ChallengesSpecification
{
    using System;
    using System.Collections.Generic;
    using Modules.Challenges.UI;
    using NUnit.Framework;

    class ChallengeDisplayInformationTests
    {
        private ChallengingDayDisplayInformationFactory _challengingDayDisplay;
        private ConsoleColor _defaultColour;
        private const char EmptySquare = '\u25A1';
        private const char FullSquare = '\u25A0';

        [SetUp]
        public void Setup()
        {
            _defaultColour = Console.ForegroundColor;
            _challengingDayDisplay = new ChallengingDayDisplayInformationFactory(_defaultColour);
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
            Assert.That(displayInformation.Colour, Is.EqualTo(_defaultColour));
        }

        [Test]
        public void ShouldReturnDrakRedFullSquareWhenAllChallengesAreFailed()
        {
            //given
            var day = new ChallengingDay
            {
                Challenges = new List<Challenge>
                {
                    new Challenge
                    {
                        Success = false
                    }
                }
            };

            //when
            var displayInformation = _challengingDayDisplay.PrepareDisplayInformation(day);

            //then
            Assert.That(displayInformation.Character, Is.EqualTo(FullSquare));
            Assert.That(displayInformation.Colour, Is.EqualTo(ConsoleColor.DarkRed));
        }

        [Test]
        public void ShouldReturnDarkGreenFullSquareWhenAllChallengesAreSuccess()
        {
            //given
            var day = new ChallengingDay
            {
                Challenges = new List<Challenge>
                {
                    new Challenge
                    {
                        Success = true
                    }
                }
            };

            //when
            var displayInformation = _challengingDayDisplay.PrepareDisplayInformation(day);

            //then
            Assert.That(displayInformation.Character, Is.EqualTo(FullSquare));
            Assert.That(displayInformation.Colour, Is.EqualTo(ConsoleColor.DarkGreen));
        }
    }
}
