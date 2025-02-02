using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using QuranCli.Arguments;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    public static class SearchHandler
    {
        public static void Handle(string[] terms, int limit, string selectionString, bool useTranslation, bool shouldIndex)
        {
            string term;
            if (Console.IsInputRedirected)
            {
                term = Console.In.ReadToEnd();
            }
            else term = string.Join(' ', terms);
            if (limit < Defaults.searchResultLimit.min || limit > Defaults.searchResultLimit.max)
            {
                throw new Exception($"The results argument should be between {Defaults.searchResultLimit.min} and {Defaults.searchResultLimit.max}");
            }
            Logger.Info($"Search term set to '{term}'.");
            IEnumerable<Verse> verses;
            if (string.IsNullOrEmpty(selectionString)) verses = Verse.SelectAll();
            else
            {
                if (!VerseSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
                verses = selection.GetVerses();
            }
            IEnumerable<Match<Verse>> matches;
            if (useTranslation) matches = Process.ExtractTop(new() { Translation = term.Strip() }, verses, verse => verse.Translation.Strip(), limit: limit).Select(match => new Match<Verse>()
            {
                Result = match.Value,
                Score = match.Score
            });
            else matches = Process.ExtractTop(new() { Text = term }, verses, verse => verse.Text, limit: limit).Select(match => new Match<Verse>()
            {
                Result = match.Value,
                Score = match.Score
            });
            if (shouldIndex) YamlProcessor.Write(IndexMatches(matches));
            else YamlProcessor.Write(matches);
        }

        private static IEnumerable<Match<Verse>> IndexMatches(IEnumerable<Match<Verse>> matches)
        {
            foreach (var match in matches)
            {
                var words = match.Result.Text.Split(' ');
                match.Result.Text = string.Join(' ', words.Select((word, index) => $"{word} [{index}]"));
                yield return match;
            }
        }
    }
}