using Fildela.Resources;
using System;

namespace Fildela.Web.Helpers
{
    public static class TimeZoneExtensions
    {
        public static DateTime GetCurrentDate()
        {
            DateTime currentTime = DateTime.UtcNow;

            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(currentTime, cstZone);

            return cstTime;
        }

        public static double GetDateDiffClient(DateTime currentDateClient)
        {
            DateTime swedishTime = GetCurrentDate();

            double hours = (swedishTime - currentDateClient).TotalHours;

            return Math.Round(hours, 0);
        }

        public static bool WithinPreviousPeriod(this DateTime dt, int daysBack)
        {
            return dt.Date > DateTime.Today.AddDays(-daysBack) && dt < DateTime.Now;
        }

        public static string GetDateWithTodayOrYesterday(DateTime date)
        {
            DateTime currentTime = GetCurrentDate();
            TimeSpan span = currentTime - date;

            if (span.Days > 1)
                return date.ToString("yyyy/MM/dd HH:mm tt");
            if (span.Days == 1)
                return String.Format("{0} {1} {2}", Resource.Yesterday, " ", date.ToString("HH:mm"));
            if (span.Hours > 0)
                return String.Format("{0} {1} {2}", Resource.Today, " ", date.ToString("HH:mm"));
            if (span.Minutes > 0)
                return String.Format("{0} {1} {2} {3} {4}", Resource.Today, " ", span.Minutes, +span.Minutes == 1 ? Resource.Minute : Resource.Minutes, Resource.Ago);
            if (span.Seconds > 10)
                return String.Format("{0} {1} {2} {3} {4}", Resource.Today, " ", span.Seconds, +span.Seconds == 1 ? Resource.Second : Resource.Seconds, Resource.Ago);
            else
                return Resource.Just_now;
        }
    }
}