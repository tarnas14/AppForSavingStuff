namespace Modules.Challenges
{
    using System;
    using UI;

    public class DisplayChallengeCommandHandler
    {
        private Cursor _challengeCursor;
        private Cursor _displayOrigin;
        private ChallengingDay[,] _displayArray;
        private Tuple<int, int> _displaySize;
        private int _displayedDaysCount;
        private readonly ChallengeRepository _challengeRepository;
        private DateTime _today = DateTime.Today;

        public DisplayChallengeCommandHandler(ChallengeRepository challengeRepository)
        {
            _challengeRepository = challengeRepository;
        }

        public void Run()
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

            var highlighter = new ChallengesGitStyleUi(uiConfiguration, new ChallengingDayDisplayInformationFactory(Console.ForegroundColor, _today));
            new WriteLineDetailDisplay(new Cursor(_displayOrigin.Left, _displayOrigin.Top + _displaySize.Item2), highlighter);

            highlighter.StartAt(_challengeCursor);
        }

        private ChallengingDay[,] PrepareChallengeDisplayArea()
        {
            var weeksToDisplay = CalculateWeeksNumberToDisplay();
            _displayedDaysCount = CalculateNumberOfDays(weeksToDisplay);
            var daysWithChallenge = _challengeRepository.GetLastDays(_displayedDaysCount, _today);

            _displaySize = Tuple.Create(weeksToDisplay, 7);

            var displayArray = new ChallengingDay[weeksToDisplay, 7];

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < weeksToDisplay; j++)
                {
                    var challengeDayIndex = j * 7 + i;

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
            var daysToDisplayInCurrentWeek = (int)_today.DayOfWeek + 1;
            var daysInPreviousWeeks = (weeksToDisplay - 1) * 7;

            return (int)(daysInPreviousWeeks + daysToDisplayInCurrentWeek);
        }

        private int CalculateWeeksNumberToDisplay()
        {
            return (int)Math.Floor((double)(Console.WindowWidth - ChallengesController.DayOfTheWeekColumnWidth - 1) / ChallengesController.WeekColumnWidth);
        }
    }
}