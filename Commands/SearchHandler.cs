using System;
using QuranCli.Arguments;
using QuranCli.Data;

namespace QuranCli.Commands
{
    internal static class SearchHandler
    {
        public static void Handle(string selectionString, string term, SearchByOption searchByOption, bool shouldTranslate, bool shouldIncludeNumbers)
        {
            if (!AyatSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            Logger.Info(selection.GetLog());
            var (ayahId1, ayahId2) = selection.GetAyahIds();
            var ayat = searchByOption == SearchByOption.Verse ?
                Repository.Instance.SearchAyahByVerse(term, ayahId1, ayahId2) :
                Repository.Instance.SearchAyahByTranslation(term, ayahId1, ayahId2);
            foreach (var line in VerseHandler.GetLines(ayat, false, shouldTranslate, shouldIncludeNumbers, 0)) Logger.Message(line);
        }
    }
}