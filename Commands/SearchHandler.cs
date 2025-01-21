using System;
using System.Linq;
using FuzzySharp;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    public static class SearchHandler
    {
        public static void Handle(string term, int results)
        {
            if (results < Defaults.searchResultLimit.min || results > Defaults.searchResultLimit.max)
            {
                throw new Exception($"The results argument should be between {Defaults.searchResultLimit.min} and {Defaults.searchResultLimit.max}");
            }
            Logger.Info($"Search term set to '{term}'.");
            var verses = Verse.SelectAll();
            var matches = Process.ExtractTop(new() { Text = term }, verses, verse => verse.Text, limit: results).Select(match => new MatchedVerse()
            {
                Verse = match.Value,
                Score = match.Score
            });
            Logger.Message(YamlSerializer.Serialze(matches));
        }
    }
}