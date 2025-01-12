using System.Linq;

namespace QuranCli.Utilities
{
    internal static class StringExtensions
    {
        public static bool IsNumeric(this string s) => s.All(char.IsDigit);

        public static bool IsSurahName(this string s) => s.All(c => char.IsLetter(c) || c == '-');

        public static bool IsSurahIdentifier(this string s) => s.IsNumeric() || s.IsSurahName();
    }
}