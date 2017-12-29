using System.Resources;

namespace Fildela.Business.Helpers
{
    public static class ResourceHelpers
    {
        public static string GetString<T>(object key)
        {
            return new ResourceManager(typeof(T)).GetString(key.ToString());
        }
    }
}
