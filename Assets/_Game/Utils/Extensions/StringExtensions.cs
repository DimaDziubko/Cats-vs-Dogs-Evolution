using System.Text.RegularExpressions;

namespace _Game.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string FixJsonArrays(this string jsonData)
        {
            return Regex.Replace(jsonData, "\"\\[([0-9,\\s]+)\\]\"", "[$1]", RegexOptions.Compiled);
        }
        
    }
}