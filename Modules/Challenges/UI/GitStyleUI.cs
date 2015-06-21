namespace Modules.Challenges.UI
{
    using System;
    using System.Text;

    public class GitStyleUI
    {
        private readonly IChallengeRepository _challengeRepository;

        public GitStyleUI(IChallengeRepository challengeRepository)
        {
            _challengeRepository = challengeRepository;
        }

        public void Run()
        {
            Console.Clear();

            var weeksToDisplay = CalculateWeeksNumberToDisplay();
            var numberOfDaysToDisplay = CalculateNumberOfDays(weeksToDisplay);
            var daysWithChallenge = _challengeRepository.GetLastDays(numberOfDaysToDisplay);

            var initialOutputEncoding = Console.OutputEncoding;
            Console.OutputEncoding = Encoding.UTF8;

            for (int i = 0; i < 7; i++)
            {
                Console.Write("{0} ", DaysOfTheWeek[i]);
                for (int j = 0; j < weeksToDisplay; j++)
                {
                    var challengeDayIndex = j*7 + i;
                    if (challengeDayIndex >= daysWithChallenge.Count)
                    {
                        continue;
                    }

                    var challenge = daysWithChallenge[challengeDayIndex];

                    if (challenge.NoChallenge)
                    {
                        Console.Write("{0} ", '\u25A1');
                    }
                }
                Console.WriteLine();
            }

            Console.OutputEncoding = initialOutputEncoding;
        }

        private string[] DaysOfTheWeek
        {
            get
            {
                return new[]
                {
                    "Sun",
                    "Mon",
                    "Tue",
                    "Wed",
                    "Thu",
                    "Fri",
                    "Sat"
                };
            }
        }

        private int CalculateNumberOfDays(double weeksToDisplay)
        {
            var daysToDisplayInCurrentWeek = (int) DateTime.Today.DayOfWeek + 1;
            var daysInPreviousWeeks = (weeksToDisplay - 1)*7;

            return (int)(daysInPreviousWeeks + daysToDisplayInCurrentWeek);
        }

        private int CalculateWeeksNumberToDisplay()
        {
            const int dayOfTheWeekColumnWidth = 4;
            const int weekColumnWidth = 2;

            return (int) Math.Floor((double)(Console.WindowWidth - dayOfTheWeekColumnWidth - 1) / weekColumnWidth);
        }
    }
}