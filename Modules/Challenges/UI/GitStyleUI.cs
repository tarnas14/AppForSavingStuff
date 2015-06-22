namespace Modules.Challenges.UI
{
    using System;
    using System.Text;

    public class GitStyleUI
    {
        private readonly IChallengeRepository _challengeRepository;
        private Tuple<int, int> _challengeCursor;
        private Tuple<int, int> _displayOrigin;
        private ChallengingDay[,] _displayArray;
        private Tuple<int, int> _displaySize;
        private int _displayedDaysCount;

        private const int WeekColumnWidth = 2;
        private const int DayOfTheWeekColumnWidth = 4;

        public GitStyleUI(IChallengeRepository challengeRepository)
        {
            _challengeRepository = challengeRepository;
            _initialEncoding = Console.OutputEncoding;
        }

        public void Run()
        {
            _displayArray = PrepareChallengeDisplayArea();

            Console.Clear();
            _displayOrigin = Tuple.Create(Console.CursorLeft, Console.CursorTop);

            Display(_displayOrigin, _displaySize, _displayArray, _displayedDaysCount);

            Highlight(_displayOrigin, _challengeCursor);

            NavigateThroughChallengeDisplay();
        }

        private static void NavigateThroughChallengeDisplay()
        {
            Console.CursorVisible = false;
            bool exit = false;
            while (!exit)
            {
                Console.ReadKey(true);
            }
        }

        private void Highlight(Tuple<int, int> displayOrigin, Tuple<int, int> challengeCursor)
        {
            Display(() =>
            {
                var topOffset = displayOrigin.Item2 + challengeCursor.Item2;
                var leftOffset = displayOrigin.Item1 + DayOfTheWeekColumnWidth + challengeCursor.Item1 * WeekColumnWidth;
                Console.CursorLeft = leftOffset;
                Console.CursorTop = topOffset;
                DisplaySelected(_displayArray[challengeCursor.Item1, challengeCursor.Item2]);
            });
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
                        _challengeCursor = Tuple.Create(j, i);
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

        private void Display(Tuple<int, int> displayOrigin, Tuple<int, int> displaySize, ChallengingDay[,] displayArray, int maxItemsToDisplay)
        {
            Display(() => {
                Console.SetCursorPosition(displayOrigin.Item1, displayOrigin.Item2);
                for (int i = 0; i < displaySize.Item2; i++)
                {
                    Console.Write("{0} ", DaysOfTheWeek[i]);
                    for (int j = 0; j < displaySize.Item1; j++)
                    {
                        if (j*7 + i >= maxItemsToDisplay)
                        {
                            continue;
                        }
                    
                        DisplayUnselected(displayArray[j,i]);
                    }
                    Console.WriteLine();
                }
            });
        }

        private void DisplaySelected(ChallengingDay challengeToDisplay)
        {
            var backgroundColour = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write("{0}", '\u25A1');
            Console.BackgroundColor = backgroundColour;
            Console.Write(' ');
        }

        private void DisplayUnselected(ChallengingDay challengeToDisplay)
        {
            Console.Write("{0} ", '\u25A1');
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

        private void Display(Action displayStuff)
        {
            var cursor = Tuple.Create(Console.CursorLeft, Console.CursorTop);
            var encoding = Console.OutputEncoding;
            Console.OutputEncoding = Encoding.UTF8;

            displayStuff();

            Console.OutputEncoding = encoding;
            Console.CursorLeft = cursor.Item1;
            Console.CursorTop = cursor.Item2;
        }
    }
}