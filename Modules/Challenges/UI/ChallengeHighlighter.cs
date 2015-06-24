namespace Modules.Challenges.UI
{
    using System;

    public class ChallengeHighlighter
    {
        private readonly Tuple<int, int> _displayOrigin;
        private readonly Tuple<int, int> _displaySize;
        private readonly ChallengingDay[,] _displayArray;
        private readonly int _displayedDaysCount;
        private Tuple<int, int> _cursor;
        private readonly ConsoleColor _background;

        public ChallengeHighlighter(Tuple<int, int> displayOrigin, Tuple<int, int> displaySize, ChallengingDay[,] displayArray, int displayedDaysCount)
        {
            _background = Console.BackgroundColor;
            _displayOrigin = displayOrigin;
            _displaySize = displaySize;
            _displayArray = displayArray;
            _displayedDaysCount = displayedDaysCount;
        }

        public void StartAt(Tuple<int, int> challengeCursor)
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
            var nextCursor = Tuple.Create(_cursor.Item1 + offsetX, _cursor.Item2 + offsetY);

            var highlightedItemId = _displaySize.Item2*nextCursor.Item1 + nextCursor.Item2;
            var tryingToMoveOutsideDisplayArea = nextCursor.Item1 < 0 || nextCursor.Item2 < 0 || highlightedItemId >= _displayedDaysCount || nextCursor.Item1 >= _displaySize.Item1 || nextCursor.Item2 >= _displaySize.Item2;
            if (tryingToMoveOutsideDisplayArea)
            {
                return;
            }

            Deselect(_cursor);
            Select(nextCursor);
            _cursor = nextCursor;
        }

        private void Deselect(Tuple<int, int> cursor)
        {
            ConsoleUtils.Utf8Display(() =>
            {
                var topOffset = _displayOrigin.Item2 + cursor.Item2;
                var leftOffset = _displayOrigin.Item1 + GitStyleChallengeUi.DayOfTheWeekColumnWidth + cursor.Item1 * GitStyleChallengeUi.WeekColumnWidth;
                Console.CursorLeft = leftOffset;
                Console.CursorTop = topOffset;
                Console.BackgroundColor = _background;
                Console.Write("{0} ", '\u25A1');
            });
        }

        private void Select(Tuple<int, int> cursor)
        {
            ConsoleUtils.Utf8Display(() =>
            {
                var topOffset = _displayOrigin.Item2 + cursor.Item2;
                var leftOffset = _displayOrigin.Item1 + GitStyleChallengeUi.DayOfTheWeekColumnWidth + cursor.Item1 * GitStyleChallengeUi.WeekColumnWidth;
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