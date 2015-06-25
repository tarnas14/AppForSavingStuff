namespace Modules.Challenges.UI
{
    using System;

    public class GitStyleChallengeUi
    {
        private readonly ChallengeRepository _challengeRepository;
        private Cursor _challengeCursor;
        private Cursor _displayOrigin;
        private ChallengingDay[,] _displayArray;
        private Tuple<int, int> _displaySize;
        private int _displayedDaysCount;

        public const int WeekColumnWidth = 2;
        public const int DayOfTheWeekColumnWidth = 4;

        public GitStyleChallengeUi(ChallengeRepository challengeRepository)
        {
            _challengeRepository = challengeRepository;
        }

        public void Run()
        {
            _displayArray = PrepareChallengeDisplayArea();

            Console.Clear();
            _displayOrigin = new Cursor(Console.CursorLeft, Console.CursorTop);

            Display(_displayOrigin, _displaySize, _displayedDaysCount);

            var uiConfiguration = new GitUiConfiguration
            {
                Origin = _displayOrigin,
                Size = _displaySize,
                DisplayedDaysCount = _displayedDaysCount,
                ChallengesArray = _displayArray
            };

            var highlighter = new ChallengeHighlighter(uiConfiguration);
            new WriteLineDetailDisplay(new Cursor(_displayOrigin.Left, _displayOrigin.Top + _displaySize.Item2), highlighter);

            highlighter.StartAt(_challengeCursor);
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

        private void Display(Cursor displayOrigin, Tuple<int, int> displaySize, int maxItemsToDisplay)
        {
            ConsoleUtils.Utf8Display(() => {
                Console.SetCursorPosition(displayOrigin.Left, displayOrigin.Top);
                for (int i = 0; i < displaySize.Item2; i++)
                {
                    Console.Write("{0} ", DaysOfTheWeek[i]);
                    for (int j = 0; j < displaySize.Item1; j++)
                    {
                        if (j*7 + i >= maxItemsToDisplay)
                        {
                            continue;
                        }

                        Console.Write("{0} ", '\u25A1');
                    }
                    Console.WriteLine();
                }
            });
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
            return (int)Math.Floor((double)(Console.WindowWidth - DayOfTheWeekColumnWidth - 1) / WeekColumnWidth);
        }
    }
}