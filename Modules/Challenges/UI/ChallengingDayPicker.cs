namespace Modules.Challenges.UI
{
    using System;

    public interface ChallengingDayPicker
    {
        event EventHandler<ChallengingDayPickedEventArgs> ChallengingDayPicked;
    }
}