using System;
using QuranCli.Arguments;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    internal static class CompareHandler
    {
        public static void Handle(string selectionString1, string selectionString2, ushort nGram)
        {
            if (!IndexedAyatSelection.TryParse(selectionString1, out var selection1)) throw new Exception("Could not parse selection");
            if (!IndexedAyatSelection.TryParse(selectionString2, out var selection2)) throw new Exception("Could not parse selection");
            var ayat1 = string.Join(' ', selection1.GetAyat());
            var ayat2 = string.Join(' ', selection2.GetAyat());
            Logger.Message(StringExtensions.JaccardSimilarity(ayat1, ayat2, nGram));
        }
    }
}