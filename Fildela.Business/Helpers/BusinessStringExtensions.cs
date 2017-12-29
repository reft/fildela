using System;
using System.Linq;

namespace Fildela.Business.Helpers
{
    public static class BusinessStringExtensions
    {
        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return string.Empty;

            return input.First().ToString().ToUpper().Trim() + input.Substring(1).ToLower().Trim();
        }
    }
}
