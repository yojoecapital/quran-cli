using System;
using System.Linq;
using QuranCli.Arguments;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    internal static class CompareHandler
    {
        public static void Handle(string selectionString1, string selectionString2, int nGram)
        {
            if (!IndexedAyatSelection.TryParse(selectionString1, out var selection1)) throw new Exception("Could not parse selection");
            if (!IndexedAyatSelection.TryParse(selectionString2, out var selection2)) throw new Exception("Could not parse selection");
            var ayat1 = string.Join(' ', selection1.GetAyat().Select(ayah => ayah.Verse));
            var ayat2 = string.Join(' ', selection2.GetAyat().Select(ayah => ayah.Verse));
            var total = Math.Max(ayat1.Length, ayat2.Length);
            if (total < Defaults.maxLevenshteinCount)
            {
                var current = StringExtensions.ComputeLevenshteinDistance(ayat1, ayat2);
                Logger.Percent(current, total, true);
            }
            else Logger.Percent(StringExtensions.JaccardSimilarity(ayat1, ayat2, nGram), true);
        }
    }
}