namespace Modules.Challenges.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using Tarnas.ConsoleUi;

    public class ChallengesController : Subscriber
    {
        private readonly GitStyleUI _gitStyleUi;

        public ChallengesController()
        {
            _gitStyleUi = new GitStyleUI(new JsonChallengeRepository());
        }

        public void Execute(UserCommand userCommand)
        {
            _gitStyleUi.Run();
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
