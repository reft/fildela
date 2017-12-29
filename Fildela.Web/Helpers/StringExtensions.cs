using System;
using System.IO;
using System.Linq;

namespace Fildela.Web.Helpers
{
    public class StringExtensions
    {
        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return string.Empty;

            return input.First().ToString().ToUpper().Trim() + input.Substring(1).ToLower().Trim();
        }

        public static string GetLastCharacters(string word, int numberOfCharacters)
        {
            if (String.IsNullOrEmpty(word) || word.Length < numberOfCharacters)
                return word;
            else
                return word.Substring(word.Length - numberOfCharacters);
        }

        public static string FirstCharToUpperRemoveTextAfterAtSign(string input)
        {
            if (String.IsNullOrEmpty(input))
                return string.Empty;

            var pos = input.LastIndexOf('@');
            if (pos >= 0)
                input = input.Substring(0, pos);

            return input.First().ToString().ToUpper().Trim() + input.Substring(1).ToLower().Trim();
        }

        public static string ShortenFileName(string fileName, int maxChars)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);

            return name.Length <= maxChars ? name : name.Substring(0, maxChars) + ".." + name[name.Length - 1] + extension;
        }

        public static string ShortenWord(string fileName, int maxChars)
        {
            return fileName.Length <= maxChars ? fileName : fileName.Substring(0, maxChars) + "..";
        }

        public static string ShortenWord(string word, int minSize, int maxSize)
        {
            if (word.Length < minSize)
                return word;

            if (String.IsNullOrEmpty(word))
                return string.Empty;

            if (word.Length < maxSize)
                maxSize = word.Length;

            word = word.Substring(0, maxSize);

            var pos = word.LastIndexOf('.');
            if (pos == -1)
                pos = word.LastIndexOf(',');
            if (pos == -1)
                pos = word.LastIndexOf('!');
            if (pos == -1)
                pos = word.LastIndexOf('?');

            if (pos >= minSize && pos <= maxSize)
                word = word.Substring(0, pos + 1);
            else
                word = word.Substring(0, maxSize).Trim() + "..";

            return word;
        }
    }
}