namespace Modules.Challenges.UI
{
    using System;

    class WriteLineDetailDisplay
    {
        private readonly Cursor _startCursor;

        public WriteLineDetailDisplay(Cursor startCursor, ChallengingDayPicker highlighter)
        {
            _startCursor = startCursor;
            highlighter.ChallengingDayPicked += DisplayDetails;
        }

        private void DisplayDetails(object sender, ChallengingDayPickedEventArgs eventArgs)
        {
            ConsoleUtils.DisplayAndReturn(() =>
            {
                Clear();
                Console.SetCursorPosition(_startCursor.Left, _startCursor.Top);
                Console.WriteLine("{0} - {1}", eventArgs.ChallengingDay.Day, eventArgs.ChallengingDay.ChallengeTitle);
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
