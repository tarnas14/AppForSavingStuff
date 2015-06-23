namespace Modules.Challenges.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using Tarnas.ConsoleUi;

    public class ChallengesController : Subscriber
    {
        private readonly GitStyleChallengeUi _gitStyleChallengeUi;

        public ChallengesController()
        {
            _gitStyleChallengeUi = new GitStyleChallengeUi(new JsonChallengeRepository());
        }

        public void Execute(UserCommand userCommand)
        {
            _gitStyleChallengeUi.Run();
        }
    }

    public class JsonChallengeRepository : IChallengeRepository
    {
        public IList<ChallengingDay> GetLastDays(int numberOfDaysToDisplay)
        {
            return Enumerable.Repeat(new ChallengingDay {NoChallenge = true}, numberOfDaysToDisplay).ToList();
        }
    }
}
