namespace Modules
{
    using System;

    public class SystemClockTimeMaster : TimeMaster
    {
        public DateTime Today
        {
            get { return DateTime.Today; }
        }

        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}