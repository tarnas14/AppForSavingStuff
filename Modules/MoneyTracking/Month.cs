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

            return new Month(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
        }
    }
}