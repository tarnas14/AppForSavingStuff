namespace Modules.Challenges.UI
{
    using System;
    using System.Linq;

    public class ChallengingDayDisplayInformationFactory
    {
        //□
        private const char EmptySquare = '\u25A1';

        public ChallengingDayDisplayInformation PrepareDisplayInformation(ChallengingDay day)
        {
            if (!day.Challenges.Any())
            {
                return new ChallengingDayDisplayInformation
                {
                    Character = EmptySquare
                };
            }

            throw new NotImplementedException();
        }
    }
}