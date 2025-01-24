using System;
using System.Collections.Generic;
using System.Linq;
using QuranCli.Arguments;
using QuranCli.Data.Models;

namespace QuranCli.Commands
{
    public static class VerseHandler
    {
        public static void Handle(string selectionString, bool shouldIndex, bool shouldTranslate, bool shouldIncludeNumbers)
        {
            if (!IndexedVerseSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            var index = 0;
            if (selection.IsIndexed && !selection.IsFromStart) index = selection.From;
            foreach (var line in GetLines(selection.GetVerses(), shouldIndex, shouldTranslate, shouldIncludeNumbers, index)) Console.WriteLine(line);
        }

        public static IEnumerable<string> GetLines(IEnumerable<Verse> verses, bool shouldIndex, bool shouldTranslate, bool shouldIncludeNumbers, int index)
        {
            foreach (var verse in verses)
            {
                string text;
                if (shouldIndex)
                {
                    index = GetNextIndex(verse.Text, index, out var indexedLine);
                    text = indexedLine;
                }
                else text = verse.Text;
                if (shouldIncludeNumbers) yield return $"{text} ({verse.Number})";
                else yield return text;
                if (shouldTranslate) yield return verse.Translation;
            }
        }

        private static int GetNextIndex(string input, int start, out string result)
        {
            var words = input.Split(' ');
            result = string.Join(" ", words.Select((word, index) => $"{word} [{start + index}]"));
            return start + words.Length;
        }

    }
}