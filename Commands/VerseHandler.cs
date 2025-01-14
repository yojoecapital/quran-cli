using System;
using System.Collections.Generic;
using System.Linq;
using QuranCli.Arguments;
using QuranCli.Data;

namespace QuranCli.Commands
{
    internal static class VerseHandler
    {
        public static void Handle(string selectionString, bool shouldIndex, bool shouldTranslate, bool shouldIncludeNumbers)
        {
            if (!IndexedAyatSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            Logger.Info(selection.GetLog());
            foreach (var line in GetLines(selection, shouldIndex, shouldTranslate, shouldIncludeNumbers)) Logger.Message(line);
            Repository.DisposeOfInstance();
        }

        public static IEnumerable<string> GetLines(IndexedAyatSelection selection, bool shouldIndex, bool shouldTranslate, bool shouldIncludeNumbers)
        {
            var index = 0;
            if (selection.IsIndexed && !selection.IsFromStart) index = selection.From;
            foreach (var ayah in selection.GetAyat())
            {
                string verse;
                if (shouldIndex)
                {
                    index = GetNextIndex(ayah.Verse, index, out var indexedLine);
                    verse = indexedLine;
                }
                else verse = ayah.Verse;
                if (shouldIncludeNumbers) yield return $"{verse} ({ayah.AyahNumber})";
                else yield return verse;
                if (shouldTranslate) yield return ayah.Translation;
            }
        }

        public static int GetNextIndex(string input, int start, out string result)
        {
            var words = input.Split(' ');
            result = string.Join(" ", words.Select((word, index) => $"{word} [{start + index}]"));
            return start + words.Length;
        }

    }
}