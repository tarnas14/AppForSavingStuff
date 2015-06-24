namespace Modules.Challenges.UI
{
    using Tarnas.ConsoleUi;

    public class ChallengesController : Subscriber
    {
        private readonly GitStyleChallengeUi _gitStyleChallengeUi;

        public ChallengesController()
        {
            _gitStyleChallengeUi = new GitStyleChallengeUi(new DummyChallengeRepository());
        }

        public void Execute(UserCommand userCommand)
        {
            _gitStyleChallengeUi.Run();
        }
    }
}
