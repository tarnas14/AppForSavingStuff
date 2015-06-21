namespace Modules.Challenges.UI
{
    using System.Collections.Generic;

    public interface IChallengeRepository
    {
        IList<ChallengingDay> GetLastDays(int numberOfDaysToDisplay);
    }
}