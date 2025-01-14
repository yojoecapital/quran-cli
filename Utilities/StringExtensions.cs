using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using QuranCli.Arguments;

namespace QuranCli.Utilities
{
    internal static class StringExtensions
    {
        public static bool IsNumeric(this string s) => s.Length > 0 && s.All(char.IsDigit);

        public static bool IsSurahName(this string s) => s.Length > 0 && s.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-');

        public static bool IsSurahIdentifier(this string s) => s.IsNumeric() || s.IsSurahName();

        public static string ExpandSelectionAnnotations(this string text)
        {
            var pattern = @"\{\{(.*?)\}\}";
            return Regex.Replace(text.Trim(), pattern, match =>
            {
                var text = match.Groups[1].Value;
                if (IndexedAyatSelection.TryParse(text, out var selection))
                {
                    var ayat = string.Join('\n', selection.GetAyat().Select(ayah => ayah.Verse));
                    return ayat;
                }
                else
                {
                    return match.Value;
                }
            });
        }

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

        public static double JaccardSimilarity(string text1, string text2, int n)
        {
            var set1 = GenerateNGrams(text1, n);
            var set2 = GenerateNGrams(text2, n);
            var intersection = set1.Intersect(set2).Count();
            var union = set1.Union(set2).Count();
            if (union == 0) return 0;
            return (double)intersection / union;
        }

        public static HashSet<string> GenerateNGrams(string text, int n)
        {
            var tokens = Tokenize(text);
            var nGrams = new HashSet<string>();
            for (int i = 0; i <= tokens.Count - n; i++)
            {
                nGrams.Add(string.Join(' ', tokens.Skip(i).Take(n)));
            }
            return nGrams;
        }

        public static HashSet<string> Tokenize(string text)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return [.. words];
        }
    }
}