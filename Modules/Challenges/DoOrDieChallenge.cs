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
        public DateTime ChallengeStart { get; set; }
        public IDictionary<DateTime, ChallengeResult> Results { get; set; }

        public DoOrDieChallenge()
        {
            Results = new Dictionary<DateTime, ChallengeResult>();
        }

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

                var challengingDay = GetChallengingDayWithResultFor(challengeDay);

                days.Add(challengingDay);
            }

            return days;
        }

        private ChallengingDay GetChallengingDayWithResultFor(DateTime challengeDay)
        {
            var challengingDay = new ChallengingDay
            {
                Day = challengeDay,
                ChallengeResult = new ChallengeResult()
            };

            ChallengeResult tmp;
            if (Results.TryGetValue(challengeDay.Date, out tmp))
            {
                challengingDay.ChallengeResult = tmp;
            }
            return challengingDay;
        }

        private const string ChallengesPath = "_challenges";

        public static DoOrDieChallenge Load(string challengeName)
        {
            var challengeFilePath = string.Format("{0}/{1}.json", ChallengesPath, challengeName);
            return
                JsonConvert.DeserializeObject<DoOrDieChallenge>(
                    File.ReadAllText(challengeFilePath));
        }

        public void MarkAsDone(DateTime date, string message = null)
        {
            Results.Add(date.Date, new ChallengeResult { Success = true, Message = message});
        }
    }
}