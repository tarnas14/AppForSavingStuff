namespace Modules.Challenges.UI
{
    using System;

    class WriteLineDetailDisplay : DetailsDisplay
    {
        private readonly Cursor _startCursor;

        public WriteLineDetailDisplay(Cursor startCursor)
        {
            _startCursor = startCursor;
        }

        public void DisplayDetails(ChallengingDay challengingDay)
        {
            ConsoleUtils.DisplayAndReturn(() =>
            {
                Clear();
                Console.SetCursorPosition(_startCursor.Left, _startCursor.Top);
                Console.WriteLine("{0} - {1}", challengingDay.Day, challengingDay.ChallengeTitle);
            });
        }

        private void Clear()
        {
            for (int i = 0; i < 1; i++)
            {
                Console.SetCursorPosition(_startCursor.Left, _startCursor.Top + i);
                ConsoleUtils.ClearLine();
            }
        }
    }
}
