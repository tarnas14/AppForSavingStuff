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

            switch (command)
            {
                case("doOrDie"):
                    HandleDoOrDieChallengeCommand(userCommand);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void HandleDoOrDieChallengeCommand(UserCommand userCommand)
        {
            if (UserWantsToBrowseChallenge(userCommand))
            {
                var challengeName = userCommand.Params[1];
                var displayChallenge = new DisplayChallengeCommandHandler(DoOrDieChallenge.Load(challengeName));
                displayChallenge.Run();
            }
        }

        private bool UserWantsToBrowseChallenge(UserCommand userCommand)
        {
            return userCommand.Params.Count == 2;
        }
    }
}