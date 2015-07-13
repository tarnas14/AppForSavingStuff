namespace Modules.Challenges
{
    using System;
    using Tarnas.ConsoleUi;

    public class ChallengesController : Subscriber
    {
        public const int WeekColumnWidth = 2;
        public const int DayOfTheWeekColumnWidth = 4;

        public void Execute(UserCommand userCommand)
        {
            var command = userCommand.Params[0];
            var challengeName = string.Empty;

            switch (command)
            {
                case("doOrDie"):
                    challengeName = userCommand.Params[1];
                    var displayChallenge = new DisplayChallengeCommandHandler(DoOrDieChallenge.Load(challengeName));
                    displayChallenge.Run();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}