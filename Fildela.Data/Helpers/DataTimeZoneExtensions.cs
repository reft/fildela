using System;

namespace Fildela.Data.Helpers
{
    public class DataTimeZoneExtensions
    {
        public static DateTime GetCurrentDate()
        {
            DateTime currentTime = DateTime.UtcNow;

            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(currentTime, cstZone);

            return cstTime;
        }
    }
}
