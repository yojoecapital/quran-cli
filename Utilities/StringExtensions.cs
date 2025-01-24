using System;
using System.Linq;

namespace QuranCli.Utilities
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string s) => s.Length > 0 && s.All(char.IsDigit);

        public static bool IsChapterTransliteration(this string s) => s.Length > 0 && s.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-');

        public static bool IsChapterIdentifier(this string s) => s.IsNumeric() || s.IsChapterTransliteration();

        public static bool IsPageIdentifier(this string s) => s[0] == 'p' && s[1..].IsNumeric();

        public static int ComputeLevenshteinDistance(string s, string t)
        {
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++) d[i, 0] = i;
            for (int j = 0; j <= n; j++) d[0, j] = j;
            for (int j = 1; j <= n; j++)
            {
                for (int i = 1; i <= m; i++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[m, n];
        }
    }
}