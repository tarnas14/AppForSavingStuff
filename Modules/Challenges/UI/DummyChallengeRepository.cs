namespace Modules.Challenges.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DummyChallengeRepository : ChallengeRepository
    {
        public IList<ChallengingDay> GetLastDays(int numberOfDaysToDisplay)
        {
            return Enumerable.Range(0, numberOfDaysToDisplay).Select(i => new ChallengingDay
            {
                ChallengeTitle = string.Format("Test challenge {0}", i),
                Day = DateTime.Today
            }).ToList();
        }
    }
}