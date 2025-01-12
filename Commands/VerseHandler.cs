using System;
using System.Collections.Generic;
using System.Linq;
using QuranCli.Arguments;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Commands
{
    internal static class VerseHandler
    {
        public static void Handle(QuranSelection selection, bool shouldIndex, bool shouldTranslate, bool shouldIncludeNumbers)
        {
            foreach (var line in GetLines(selection, shouldIndex, shouldTranslate, shouldIncludeNumbers)) Console.WriteLine(line);
        }

        public static IEnumerable<string> GetLines(QuranSelection selection, bool shouldIndex, bool shouldTranslate, bool shouldIncludeNumbers)
        {
            var index = 0;
            var number = 0;
            foreach (var ayah in selection.GetAyat(Repository.Instance))
            {
                string verse;
                if (shouldIndex)
                {
                    index = GetNextIndex(ayah.Verse, index, out var indexedLine);
                    verse = indexedLine;
                }
                else verse = ayah.Verse;
                if (shouldIncludeNumbers) yield return $"{verse} ({number})";
                else yield return verse;
                if (shouldTranslate) yield return ayah.Translation;
                number++;
            }
        }

        public static int GetNextIndex(string input, int start, out string result)
        {
            var words = input.Split(' ');
            result = string.Join(" ", words.Select((word, index) => $"[{start + index}] {word}"));
            return start + words.Length;
        }

    }
}