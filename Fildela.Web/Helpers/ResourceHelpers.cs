using System.Resources;
using System.Web.Mvc;

namespace Fildela.Web.Helpers
{
    public static class ResourceHelpers
    {
        public static string GetStringFromView<T>(this HtmlHelper html, object key)
        {
            return new ResourceManager(typeof(T)).GetString(key.ToString());
        }

        public static string GetString<T>(object key)
        {
            return new ResourceManager(typeof(T)).GetString(key.ToString());
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                