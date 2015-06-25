namespace Modules.Challenges.UI
{
    using System;

    public class GitUiConfiguration
    {
        public Cursor Origin { get; set; }
        public Tuple<int, int> Size { get; set; }
        public int DisplayedDaysCount { get; set; }
        public ChallengingDay[,] ChallengesArray { get; set; }
    }
}