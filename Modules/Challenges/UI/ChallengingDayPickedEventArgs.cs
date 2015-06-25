namespace Modules.Challenges.UI
{
    using System;

    public class ChallengingDayPickedEventArgs : EventArgs
    {
        public ChallengingDay ChallengingDay { get; private set; }
        
        public ChallengingDayPickedEventArgs(ChallengingDay challenge)
        {
            ChallengingDay = challenge;
        }
    }
}