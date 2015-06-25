namespace Modules.Challenges
{
    using System;
    using Tarnas.ConsoleUi;
    using UI;
    using Console = System.Console;

    public class ChallengesController : Subscriber
    {
        private readonly ChallengeRepository _challengeRepository;
        private Cursor _challengeCursor;
        private Cursor _displayOrigin;
        private ChallengingDay[,] _displayArray;
        private Tuple<int, int> _displaySize;
        private int _displayedDaysCount;

        public const int WeekColumnWidth = 2;
        public const int DayOfTheWeekColumnWidth = 4;

        public void Execute(UserCommand userCommand)
        {
            _displayArray = PrepareChallengeDisplayArea();

            Console.Clear();
            _displayOrigin = new Cursor(Console.CursorLeft, Console.CursorTop);

            var uiConfiguration = new GitUiConfiguration
            {
                Origin = _displayOrigin,
                Size = _displaySize,
                DisplayedDaysCount = _displayedDaysCount,
                ChallengesArray = _displayArray
            };

            var highlighter = new ChallengesGitStyleUi(uiConfiguration);
            new WriteLineDetailDisplay(new Cursor(_displayOrigin.Left, _displayOrigin.Top + _displaySize.Item2), highlighter);

            highlighter.StartAt(_challengeCursor);
        }

        public ChallengesController(ChallengeRepository challengeRepository)
        {
            _challengeRepository = challengeRepository;
        }

        private ChallengingDay[,] PrepareChallengeDisplayArea()
        {
            var weeksToDisplay = CalculateWeeksNumberToDisplay();
            _displayedDaysCount = CalculateNumberOfDays(weeksToDisplay);
            var daysWithChallenge = _challengeRepository.GetLastDays(_displayedDaysCount);

            _displaySize = Tuple.Create(weeksToDisplay, 7);

            var displayArray = new ChallengingDay[weeksToDisplay, 7];

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < weeksToDisplay; j++)
                {
                    var challengeDayIndex = j*7 + i;

                    if (challengeDayIndex == daysWithChallenge.Count - 1)
                    {
                        _challengeCursor = new Cursor(j, i);
                    }

                    if (challengeDayIndex >= daysWithChallenge.Count)
                    {
                        continue;
                    }

                    displayArray[j, i] = daysWithChallenge[challengeDayIndex];
                }
            }

            return displayArray;
        }

        private int CalculateNumberOfDays(double weeksToDisplay)
        {
            var daysToDisplayInCurrentWeek = (int) DateTime.Today.DayOfWeek + 1;
            var daysInPreviousWeeks = (weeksToDisplay - 1)*7;

            return (int)(daysInPreviousWeeks + daysToDisplayInCurrentWeek);
        }

        private int CalculateWeeksNumberToDisplay()
        {
            return (int)Math.Floor((double)(Console.WindowWidth - DayOfTheWeekColumnWidth - 1) / WeekColumnWidth);
        }
    }
}