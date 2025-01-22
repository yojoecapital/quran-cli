using System;
using System.Collections.Generic;
using QuranCli.Arguments;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    public static class MarkdownProcessor
    {
        public static IEnumerable<ColoredString> GetColoredStrings(string text)
        {
            foreach (var line in text.Split(Environment.NewLine))
            {
                if (IsHeader(line)) yield return new(line, ConsoleColor.Magenta);
                else
                {
                    foreach (var coloredString in GetColoredStringsFromLine(line)) yield return coloredString;
                }
                yield return new(Environment.NewLine, null);
            }
        }

        public static IEnumerable<(int id1, int id2)> GetReferences(string text)
        {
            foreach (var line in text.Split(Environment.NewLine))
            {
                if (IsHeader(line)) continue;
                foreach (var reference in GetReferencesFromLine(line)) yield return reference;
            }
        }

        private static IEnumerable<(int id1, int id2)> GetReferencesFromLine(string line)
        {
            if (line.Length == 0) yield break;
            else if (MatchesStart(line, "#", out var before, out var match, out var after) && VerseSelection.TryParse(match[1..], out var selection))
            {
                foreach (var reference in GetReferencesFromLine(before)) yield return reference;
                yield return selection.GetVerseIds();
                foreach (var reference in GetReferencesFromLine(after)) yield return reference;

            }
            else if (MatchesBetween(line, "{", "}", out before, out match, out after) && IndexedVerseSelection.TryParse(match[1..^1], out var indexedSelection))
            {
                foreach (var reference in GetReferencesFromLine(before)) yield return reference;
                yield return indexedSelection.GetVerseIds();
                foreach (var reference in GetReferencesFromLine(after)) yield return reference;
            }
        }

        private static IEnumerable<ColoredString> GetColoredStringsFromLine(string line)
        {
            if (line.Length == 0) yield break;
            else if (MatchesStart(line, "#", out var before, out var match, out var after) && VerseSelection.TryParse(match[1..], out var selection))
            {
                foreach (var coloredString in GetColoredStringsFromLine(before)) yield return coloredString;
                var (id1, id2) = selection.GetVerseIds();
                yield return new(Verse.GetDisplayName(id1, id2), ConsoleColor.Green);
                foreach (var coloredString in GetColoredStringsFromLine(after)) yield return coloredString;
            }
            else if (MatchesBetween(line, "{", "}", out before, out match, out after) && IndexedVerseSelection.TryParse(match[1..^1], out var indexedSelection))
            {
                foreach (var coloredString in GetColoredStringsFromLine(before)) yield return coloredString;
                if (indexedSelection.isChapterSelection && !indexedSelection.IsIndexed)
                {
                    yield return new(Chapter.GetDisplayName(indexedSelection.chapterNumber1, indexedSelection.chapterNumber2), ConsoleColor.Green);
                }
                else
                {
                    foreach (var verse in indexedSelection.GetVerses())
                    {
                        yield return new($"{verse.Text} ({verse.Number})", ConsoleColor.Green);
                    }
                }
                foreach (var coloredString in GetColoredStringsFromLine(after)) yield return coloredString;
            }
            else if (MatchesBetween(line, "**", "**", out before, out match, out after))
            {
                foreach (var coloredString in GetColoredStringsFromLine(before)) yield return coloredString;
                yield return new(match, ConsoleColor.Yellow);
                foreach (var coloredString in GetColoredStringsFromLine(after)) yield return coloredString;
            }
            else if (MatchesBetween(line, "*", "*", out before, out match, out after))
            {
                foreach (var coloredString in GetColoredStringsFromLine(before)) yield return coloredString;
                yield return new(match, ConsoleColor.Blue);
                foreach (var coloredString in GetColoredStringsFromLine(after)) yield return coloredString;
            }
            else if (MatchesBetween(line, "`", "`", out before, out match, out after))
            {
                foreach (var coloredString in GetColoredStringsFromLine(before)) yield return coloredString;
                yield return new(match, ConsoleColor.Cyan);
                foreach (var coloredString in GetColoredStringsFromLine(after)) yield return coloredString;
            }
            else yield return new(line, null);
        }

        private static bool MatchesBetween(string input, string start, string end, out string before, out string match, out string after)
        {
            before = match = after = input;
            var startIndex = input.IndexOf(start);
            if (startIndex == -1) return false;
            var endIndex = input.IndexOf(end, startIndex + start.Length);
            if (endIndex == -1) return false;
            match = input[startIndex..(endIndex + end.Length)];
            if (
                match.Length <= start.Length + end.Length ||
                !IsWordCharacter(match[start.Length]) ||
                !IsWordCharacter(match[match.Length - end.Length - 1])
            ) return false;
            before = input[..startIndex];
            after = input[(endIndex + end.Length)..];
            return true;
        }

        private static bool MatchesStart(string input, string start, out string before, out string match, out string after)
        {
            before = match = after = input;
            var startIndex = input.IndexOf(start);
            if (startIndex == -1) return false;
            var endIndex = startIndex + 1;
            while (endIndex < input.Length && IsWordCharacter(input[endIndex]))
            {
                endIndex++;
            }
            if (endIndex == startIndex + 1 || (endIndex != input.Length && !IsWordCharacter(input[endIndex - 1])))
            {
                return false;
            }
            match = input[startIndex..endIndex];
            before = input[..startIndex];
            after = input[endIndex..];
            return true;
        }

        private static bool IsHeader(string input)
        {
            var startIndex = 0;
            while (startIndex < input.Length && char.IsWhiteSpace(input[startIndex]))
            {
                startIndex++;
            }
            if (startIndex >= input.Length || input[startIndex] != '#') return false;
            var headerLevel = 0;
            while (headerLevel < 6 && startIndex + headerLevel < input.Length && input[startIndex + headerLevel] == '#')
            {
                headerLevel++;
            }
            var spaceIndex = startIndex + headerLevel;
            if (spaceIndex >= input.Length || input[spaceIndex] != ' ') return false;
            spaceIndex++;
            while (spaceIndex < input.Length && char.IsWhiteSpace(input[spaceIndex]))
            {
                spaceIndex++;
            }
            if (spaceIndex >= input.Length || input[spaceIndex] == ' ') return false;
            return true;
        }

        private static bool IsWordCharacter(char c) => char.IsLetterOrDigit(c) || char.IsPunctuation(c) || c == '-';
    }
}