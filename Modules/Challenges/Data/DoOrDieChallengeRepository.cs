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

        public IList<ChallengingDay> GetLastDays(int numberOfDaysToDisplay, DateTime today)
        {
            var days = new List<ChallengingDay>();

            for (int i = numberOfDaysToDisplay - 1; i >= 0; i--)
            {
                var challengeDay = today.Subtract(TimeSpan.FromDays(i));

                if (challengeDay < _doOrDieChallenge.ChallengeStart)
                {
                    days.Add(new ChallengingDay
                    {
                        Day = challengeDay,
                    });
                    continue;
                }

                days.Add(new ChallengingDay
                {
                    Day = challengeDay,
                    Challenges = _doOrDieChallenge.Definition
                });
            }

            return days;
        }
    }
}