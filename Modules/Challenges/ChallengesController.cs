namespace Modules.Challenges
{
    using System;
    using Data;
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
                    var challengeName = userCommand.Params[1];
                    var displayChallenge = new DisplayChallengeCommandHandler(new DoOrDieChallengeRepository(ChallengeDefinitionReader.ReadDoOrDieDefinition(challengeName)));
                    displayChallenge.Run();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
       
    }
}