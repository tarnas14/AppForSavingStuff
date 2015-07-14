namespace Modules.Challenges.UI
{
    using System;
    using System.Linq;

    public class ChallengingDayDisplayInformationFactory
    {
        private readonly ConsoleColor _defaultColour;
        private readonly DateTime _today;

        public ChallengingDayDisplayInformationFactory(ConsoleColor defaultColour, DateTime today)
        {
            _defaultColour = defaultColour;
            _today = today;
        }

        //□
        private const char EmptySquare = '\u25A1';
        //■
        private const char FullSquare = '\u25A0';

        public ChallengingDayDisplayInformation PrepareDisplayInformation(ChallengingDay day)
        {
            if (day.ChallengeResult == null || TodayNotASuccess(day))
            {
                return new ChallengingDayDisplayInformation
                {
                    Character = EmptySquare,
                    Colour = _defaultColour
                };
            }

            if (!day.ChallengeResult.Success)
            {
                return new ChallengingDayDisplayInformation
                {
                    Character = FullSquare,
                    Colour = ConsoleColor.DarkRed
                };
            }

            if (day.ChallengeResult.Success)
            {
                return new ChallengingDayDisplayInformation
                {
                    Character = FullSquare,
                    Colour = ConsoleColor.DarkGreen
                };
            }

            return new ChallengingDayDisplayInformation
            {
                Character = ' ',
                Colour = _defaultColour
            };
        }

        private bool TodayNotASuccess(ChallengingDay day)
        {
            return day.Day.Date == _today.Date && !day.ChallengeResult.Success;
        }
    }
}