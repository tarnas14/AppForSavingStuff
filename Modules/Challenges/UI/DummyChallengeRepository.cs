namespace Modules.Challenges.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DummyChallengeRepository : ChallengeRepository
    {
        private readonly Random _random;

        public DummyChallengeRepository()
        {
            _random = new Random();
        }

        public IList<ChallengingDay> GetLastDays(int numberOfDaysToDisplay, DateTime today)
        {
            return Enumerable.Range(0, numberOfDaysToDisplay).Select(i => new ChallengingDay
            {
                Day = today,
                ChallengeResult = _random.Next(0, 100) > 49 ? null : GetRandomChallenge()
            }).ToList();
        }

        private ChallengeResult GetRandomChallenge()
        {
            return new ChallengeResult
            {
                Success = _random.Next(0, 100) > 49,
                Message = "dummy description here"
            };
        }
    }
}