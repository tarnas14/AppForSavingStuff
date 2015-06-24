namespace Modules.Challenges.UI
{
    using System.Collections.Generic;

    public interface ChallengeRepository
    {
        IList<ChallengingDay> GetLastDays(int numberOfDaysToDisplay);
    }
}