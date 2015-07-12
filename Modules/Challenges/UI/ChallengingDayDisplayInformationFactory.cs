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
            if (!day.Challenges.Any() || day.Day.Date == _today.Date)
            {
                return new ChallengingDayDisplayInformation
                {
                    Character = EmptySquare,
                    Colour = _defaultColour
                };
            }

            if (day.Challenges.All(challenge => !challenge.Success))
            {
                return new ChallengingDayDisplayInformation
                {
                    Character = FullSquare,
                    Colour = ConsoleColor.DarkRed
                };
            }

            if (day.Challenges.All(challenge => challenge.Success))
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
    }
}