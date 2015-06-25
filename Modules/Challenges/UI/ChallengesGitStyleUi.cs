namespace Modules.Challenges.UI
{
    using System;

    public class ChallengesGitStyleUi : ChallengingDayPicker
    {
        public event EventHandler<ChallengingDayPickedEventArgs> ChallengingDayPicked;

        private readonly GitUiConfiguration _uiConfiguration;
        private readonly ConsoleColor _background;
        private Cursor _cursor;
        private readonly ChallengingDayDisplayInformationFactory _dayDisplayInformationFactory;

        public ChallengesGitStyleUi(GitUiConfiguration uiConfiguration)
        {
            _uiConfiguration = uiConfiguration;
            _dayDisplayInformationFactory = new ChallengingDayDisplayInformationFactory(Console.ForegroundColor);
            Display();
        }

        private void Display()
        {
            ConsoleUtils.Utf8Display(() =>
            {
                Console.SetCursorPosition(_uiConfiguration.Origin.Left, _uiConfiguration.Origin.Top);
                for (int i = 0; i < _uiConfiguration.Size.Item2; i++)
                {
                    Console.Write("{0} ", DaysOfTheWeek[i]);
                    for (int j = 0; j < _uiConfiguration.Size.Item1; j++)
                    {
                        if (j * 7 + i >= _uiConfiguration.DisplayedDaysCount)
                        {
                            continue;
                        }

                        ConsoleUtils.Display(GetDayDisplayInformation(j, i));
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                }
            });
        }

        private ChallengingDayDisplayInformation GetDayDisplayInformation(int x, int y)
        {
            var day = _uiConfiguration.ChallengesArray[x, y];
            return _dayDisplayInformationFactory.PrepareDisplayInformation(day);
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

        public void StartAt(Cursor challengeCursor)
        {
            _cursor = challengeCursor;

            Select(challengeCursor);

            Run();
        }

        private void Run()
        {
            Console.CursorVisible = false;
            while (true)
            {
                var consoleKeyInfo = Console.ReadKey(true);

                if (consoleKeyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }

                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        MoveCursor(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        MoveCursor(1, 0);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveCursor(0, 1);
                        break;
                    case ConsoleKey.UpArrow:
                        MoveCursor(0, -1);
                        break;
                }
            }
            Console.CursorVisible = true;
        }

        private void MoveCursor(int offsetX, int offsetY)
        {
            var nextCursor = new Cursor(_cursor.Left + offsetX, _cursor.Top + offsetY);

            var highlightedItemId = _uiConfiguration.Size.Item2 * nextCursor.Left + nextCursor.Top;
            var tryingToMoveOutsideDisplayArea = 
                nextCursor.Left < 0 || 
                nextCursor.Top < 0 || 
                highlightedItemId >= _uiConfiguration.DisplayedDaysCount ||
                nextCursor.Left >= _uiConfiguration.Size.Item1 ||
                nextCursor.Top >= _uiConfiguration.Size.Item2;

            if (tryingToMoveOutsideDisplayArea)
            {
                return;
            }

            Deselect(_cursor);
            Select(nextCursor);
            _cursor = nextCursor;
        }

        private void Deselect(Cursor cursor)
        {
            ConsoleUtils.Utf8Display(() =>
            {
                var topOffset = _uiConfiguration.Origin.Top + cursor.Top;
                var leftOffset = _uiConfiguration.Origin.Left + ChallengesController.DayOfTheWeekColumnWidth + cursor.Left * ChallengesController.WeekColumnWidth;
                Console.CursorLeft = leftOffset;
                Console.CursorTop = topOffset;
                Console.BackgroundColor = _background;
                ConsoleUtils.Display(GetDayDisplayInformation(cursor.Left, cursor.Top));
                Console.Write(' ');
            });
        }

        private void Select(Cursor cursor)
        {
            DisplayDetails(_uiConfiguration.ChallengesArray[cursor.Left, cursor.Top]);
            ConsoleUtils.Utf8Display(() =>
            {
                var topOffset = _uiConfiguration.Origin.Top + cursor.Top;
                var leftOffset = _uiConfiguration.Origin.Left + ChallengesController.DayOfTheWeekColumnWidth + cursor.Left * ChallengesController.WeekColumnWidth;
                Console.CursorLeft = leftOffset;
                Console.CursorTop = topOffset;
                var backgroundColour = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                ConsoleUtils.Display(GetDayDisplayInformation(cursor.Left, cursor.Top));
                Console.BackgroundColor = backgroundColour;
                Console.Write(' ');
            });
        }

        private void DisplayDetails(ChallengingDay challengingDay)
        {
            if (ChallengingDayPicked != null)
            {
                ChallengingDayPicked(this, new ChallengingDayPickedEventArgs(challengingDay));
            }
        }
    }
}