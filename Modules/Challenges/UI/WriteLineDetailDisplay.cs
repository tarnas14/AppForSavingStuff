namespace Modules.Challenges.UI
{
    using System;
    using System.Linq;

    class WriteLineDetailDisplay
    {
        private readonly Cursor _startCursor;
        private int _lastDisplayedChallengesCount = 0;

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
                foreach (var challenge in eventArgs.ChallengingDay.Challenges)
                {
                    Console.WriteLine(challenge.Description);
                }
                _lastDisplayedChallengesCount = eventArgs.ChallengingDay.Challenges.Count();
            });
        }

        private void Clear()
        {
            for (int i = 0; i < 1 + _lastDisplayedChallengesCount; i++)
            {
                Console.SetCursorPosition(_startCursor.Left, _startCursor.Top + i);
                ConsoleUtils.ClearLine();
            }
        }
    }
}
