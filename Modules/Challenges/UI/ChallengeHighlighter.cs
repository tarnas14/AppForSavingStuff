namespace Modules.Challenges.UI
{
    using System;

    public class ChallengeHighlighter
    {
        private readonly Cursor _displayOrigin;
        private readonly Tuple<int, int> _displaySize;
        private readonly ChallengingDay[,] _displayArray;
        private readonly int _displayedDaysCount;
        private Cursor _cursor;
        private readonly ConsoleColor _background;
        private readonly DetailsDisplay _detailsDisplay;

        public ChallengeHighlighter(Cursor displayOrigin, Tuple<int, int> displaySize, ChallengingDay[,] displayArray, int displayedDaysCount, DetailsDisplay detailsDisplay)
        {
            _background = Console.BackgroundColor;
            _displayOrigin = displayOrigin;
            _displaySize = displaySize;
            _displayArray = displayArray;
            _displayedDaysCount = displayedDaysCount;
            _detailsDisplay = detailsDisplay;
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

            var highlightedItemId = _displaySize.Item2*nextCursor.Left + nextCursor.Top;
            var tryingToMoveOutsideDisplayArea = 
                nextCursor.Left < 0 || 
                nextCursor.Top < 0 || 
                highlightedItemId >= _displayedDaysCount || 
                nextCursor.Left >= _displaySize.Item1 || 
                nextCursor.Top >= _displaySize.Item2;

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
                var topOffset = _displayOrigin.Top + cursor.Top;
                var leftOffset = _displayOrigin.Left + GitStyleChallengeUi.DayOfTheWeekColumnWidth + cursor.Left * GitStyleChallengeUi.WeekColumnWidth;
                Console.CursorLeft = leftOffset;
                Console.CursorTop = topOffset;
                Console.BackgroundColor = _background;
                Console.Write("{0} ", '\u25A1');
            });
        }

        private void Select(Cursor cursor)
        {
            _detailsDisplay.DisplayDetails(_displayArray[cursor.Left, cursor.Top]);
            ConsoleUtils.Utf8Display(() =>
            {
                var topOffset = _displayOrigin.Top + cursor.Top;
                var leftOffset = _displayOrigin.Left + GitStyleChallengeUi.DayOfTheWeekColumnWidth + cursor.Left * GitStyleChallengeUi.WeekColumnWidth;
                Console.CursorLeft = leftOffset;
                Console.CursorTop = topOffset;
                var backgroundColour = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("{0}", '\u25A1');
                Console.BackgroundColor = backgroundColour;
                Console.Write(' ');
            });
        }
    }
}