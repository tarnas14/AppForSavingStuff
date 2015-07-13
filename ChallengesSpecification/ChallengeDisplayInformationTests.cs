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
        private readonly DateTime _today = new DateTime(2015, 11, 23);

        [SetUp]
        public void Setup()
        {
            _defaultColour = Console.ForegroundColor;
            _challengingDayDisplay = new ChallengingDayDisplayInformationFactory(_defaultColour, _today);
        }

        [Test]
        public void ShouldReturnEmptySquareForDaysWithoutChallenge()
        {
            //given
            var day = new ChallengingDay
            {
                ChallengeResult = null
            };

            //when
            var displayInformation = _challengingDayDisplay.PrepareDisplayInformation(day);

            //then
            Assert.That(displayInformation.Character, Is.EqualTo(EmptySquare));
            Assert.That(displayInformation.Colour, Is.EqualTo(_defaultColour));
        }

        [Test]
        public void ShouldReturnDarkRedFullSquareWhenChallengeIsNotASuccess()
        {
            //given
            var day = new ChallengingDay
            {
                ChallengeResult = new ChallengeResult
                {
                    Success = false
                }
            };

            //when
            var displayInformation = _challengingDayDisplay.PrepareDisplayInformation(day);

            //then
            Assert.That(displayInformation.Character, Is.EqualTo(FullSquare));
            Assert.That(displayInformation.Colour, Is.EqualTo(ConsoleColor.DarkRed));
        }

        [Test]
        public void ShouldReturnDarkGreenFullSquareWhenChallengeIsASuccess()
        {
            //given
            var day = new ChallengingDay
            {
                ChallengeResult = new ChallengeResult
                    {
                        Success = true
                    }
            };

            //when
            var displayInformation = _challengingDayDisplay.PrepareDisplayInformation(day);

            //then
            Assert.That(displayInformation.Character, Is.EqualTo(FullSquare));
            Assert.That(displayInformation.Colour, Is.EqualTo(ConsoleColor.DarkGreen));
        }

        [Test]
        public void ShouldReturnEmptySquareForTodayEvenIfItIsNotASuccessYet()
        {
            //given
            var day = new ChallengingDay
            {
                Day = _today,
                ChallengeResult = new ChallengeResult
                    {
                        Success = false
                    }
            };

            //when
            var displayInformation = _challengingDayDisplay.PrepareDisplayInformation(day);

            //then
            Assert.That(displayInformation.Character, Is.EqualTo(EmptySquare));
            Assert.That(displayInformation.Colour, Is.EqualTo(_defaultColour));
        }

        [Test]
        public void ShouldReturnFullDarkGreenSquareForTodayIfItIsASuccess()
        {
            //given
            var day = new ChallengingDay
            {
                Day = _today,
                ChallengeResult = new ChallengeResult
                    {
                        Success = true
                    }
            };

            //when
            var displayInformation = _challengingDayDisplay.PrepareDisplayInformation(day);

            //then
            Assert.That(displayInformation.Character, Is.EqualTo(EmptySquare));
            Assert.That(displayInformation.Colour, Is.EqualTo(_defaultColour));
        }
    }
}
