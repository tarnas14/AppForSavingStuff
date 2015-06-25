namespace Modules.Challenges.UI
{
    using System;

    public class ChallengeHighlighter : ChallengingDayPicker
    {
        public event EventHandler<ChallengingDayPickedEventArgs> ChallengingDayPicked;

        private readonly GitUiConfiguration _uiConfiguration;
        private readonly ConsoleColor _background;
        private Cursor _cursor;

        public ChallengeHighlighter(GitUiConfiguration uiConfiguration)
        {
            _uiConfiguration = uiConfiguration;
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
                var leftOffset = _uiConfiguration.Origin.Left + GitStyleChallengeUi.DayOfTheWeekColumnWidth + cursor.Left * GitStyleChallengeUi.WeekColumnWidth;
                Console.CursorLeft = leftOffset;
                Console.CursorTop = topOffset;
                Console.BackgroundColor = _background;
                Console.Write("{0} ", '\u25A1');
            });
        }

        private void Select(Cursor cursor)
        {
            DisplayDetails(_uiConfiguration.ChallengesArray[cursor.Left, cursor.Top]);
            ConsoleUtils.Utf8Display(() =>
            {
                var topOffset = _uiConfiguration.Origin.Top + cursor.Top;
                var leftOffset = _uiConfiguration.Origin.Left + GitStyleChallengeUi.DayOfTheWeekColumnWidth + cursor.Left * GitStyleChallengeUi.WeekColumnWidth;
                Console.CursorLeft = leftOffset;
                Console.CursorTop = topOffset;
                var backgroundColour = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("{0}", '\u25A1');
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