using System;

namespace Fildela.Data.Helpers
{
    public class DataGuidExtensions
    {
        public static string DateAndGuid()
        {
            string guid = DataTimeZoneExtensions.GetCurrentDate().ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N").ToUpper();

            return guid;
        }
    }
}                                                                                                                                                                                                                                                                                                                                                     