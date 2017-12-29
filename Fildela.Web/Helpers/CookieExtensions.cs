using System;
using System.Web;

namespace Fildela.Web.Helpers
{
    public class CookieExtensions
    {
        internal static void createCookie(string name, int daysBeforeExpire, string value, bool httpOnly)
        {
            HttpCookie cookie = new HttpCookie(name);
            DateTime now = TimeZoneExtensions.GetCurrentDate();
            cookie.Expires = now.AddDays(daysBeforeExpire);
            cookie.HttpOnly = httpOnly;
            cookie.Value = value;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}