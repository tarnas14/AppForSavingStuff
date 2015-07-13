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
                Console.WriteLine(eventArgs.ChallengingDay.Day);
                Console.WriteLine(eventArgs.ChallengingDay.ChallengeResult.Message);
            });
        }

        private void Clear()
        {
            for (int i = 0; i < 2; i++)
            {
                Console.SetCursorPosition(_startCursor.Left, _startCursor.Top + i);
                ConsoleUtils.ClearLine();
            }
        }
    }
}
