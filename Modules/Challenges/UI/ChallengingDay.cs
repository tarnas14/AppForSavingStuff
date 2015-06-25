namespace Modules.Challenges.UI
{
    using System;
    using System.Collections.Generic;

    public class ChallengingDay
    {
        public ChallengingDay()
        {
            Challenges = new List<Challenge>();
        }

        public DateTime Day { get; set; }
        public string ChallengeTitle { get; set; }
        public IEnumerable<Challenge> Challenges { get; set; }
    }
}