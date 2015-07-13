namespace Modules.Challenges
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using UI;

    public class DoOrDieChallenge
    {
        public string Name { get; set; }
        public ICollection<int> Cycle { get; set; }
        public ICollection<Challenge> Definition { get; set; }
        public DateTime ChallengeStart { get; set; }

        public IList<ChallengingDay> GetLastDays(int numberOfDaysToDisplay, DateTime today)
        {
            var days = new List<ChallengingDay>();

            for (int i = numberOfDaysToDisplay - 1; i >= 0; i--)
            {
                var challengeDay = today.Subtract(TimeSpan.FromDays(i));

                if (challengeDay < ChallengeStart)
                {
                    days.Add(new ChallengingDay
                    {
                        Day = challengeDay,
                    });
                    continue;
                }

                days.Add(new ChallengingDay
                {
                    Day = challengeDay,
                    Challenges = Definition
                });
            }

            return days;
        }

        private const string ChallengesPath = "_challenges";

        public static DoOrDieChallenge Load(string challengeName)
        {
            var challengeFilePath = string.Format("{0}/{1}.json", ChallengesPath, challengeName);
            return
                JsonConvert.DeserializeObject<DoOrDieChallenge>(
                    File.ReadAllText(challengeFilePath));
        }
    }
}