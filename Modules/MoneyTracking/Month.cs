namespace Modules.MoneyTracking
{
    using System;

    public class Month
    {
        public int Year { get; private set; }
        public int MonthNr { get; private set; }

        public Month(int year, int month)
        {
            Year = year;
            MonthNr = month;
        }

        public static Month FromString(string monthParam)
        {
            var parts = monthParam.Split('-');

            int yearh = Convert.ToInt32(parts[0]);
            int month = Convert.ToInt32(parts[1]);
            return new Month(yearh, month);
        }

        public string GetIndexString()
        {
            return string.Format("{0,2:00}{1,2:00}", MonthNr, Year - 2000); //very ugly hack, but will work for 985 years to come, so whatever
        }

        public static Month FromToday(TimeMaster timeMaster)
        {
            return new Month(timeMaster.Today.Year, timeMaster.Today.Month);
        }
    }
}