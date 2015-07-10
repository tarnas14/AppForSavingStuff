namespace Modules.Challenges.Data
{
    using System;
    using System.Collections.Generic;
    using UI;

    public class DoOrDieChallengeRepository : ChallengeRepository
    {
        private readonly DoOrDieChallengeDefinition _doOrDieChallenge;

        public DoOrDieChallengeRepository(DoOrDieChallengeDefinition doOrDieChallenge)
        {
            _doOrDieChallenge = doOrDieChallenge;
        }

        public IList<ChallengingDay> GetLastDays(int numberOfDaysToDisplay)
        {
            var days = new List<ChallengingDay>();

            var today = DateTime.Today;
            for (int i = numberOfDaysToDisplay - 1; i >= 0; i--)
            {
                days.Add(new ChallengingDay
                {
                    Day = today.Subtract(TimeSpan.FromDays(i)),
                    ChallengeTitle = _doOrDieChallenge.Name,
                    Challenges = _doOrDieChallenge.Definition
                });
            }

            return days;
        }
    }
}