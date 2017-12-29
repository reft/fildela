using System;

namespace Fildela.Web.Helpers
{
    public class GuidExtensions
    {
        public static string DateAndGuid()
        {
            string guid = TimeZoneExtensions.GetCurrentDate().ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N");

            return guid;
        }
    }
}                                                                                                                                                                                                                                                                                                                                                     