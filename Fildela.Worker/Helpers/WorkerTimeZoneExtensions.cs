using System;

namespace Fildela.Worker.Helpers
{
    public class WorkerTimeZoneExtensions
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
