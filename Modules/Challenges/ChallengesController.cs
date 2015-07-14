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
                case("doOrDieDone"):
                    challengeName = userCommand.Params[1];
                    var doOrDieChallenge = DoOrDieChallenge.Load(challengeName);
                    var message = string.Empty;
                    if (userCommand.Params.Count == 3)
                    {
                        message = userCommand.Params[2];
                    }

                    doOrDieChallenge.MarkAsDone(DateTime.Today, message);
                    doOrDieChallenge.Save();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}