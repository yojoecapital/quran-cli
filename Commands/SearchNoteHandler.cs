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
    public static class SearchNoteHandler
    {
        public static void Handle(string[] terms, int limit, string selectionString)
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
            IEnumerable<Match<Note>> matches;
            if (string.IsNullOrEmpty(selectionString))
            {
                var notes = Note.SelectAll();
                matches = Process.ExtractTop(new() { Text = term.Strip() }, notes, note => note.Text.Strip(), limit: limit).Select(match => new Match<Note>()
                {
                    Result = match.Value,
                    Score = match.Score
                });
            }
            else
            {
                if (!VerseSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
                var verseId1 = selection.VerseId1;
                var verseId2 = selection.VerseId2;
                var notes = Reference.SelectBetween(verseId1, verseId1).DistinctBy(reference => reference.NoteId).Select(reference => Note.SelectById(reference.NoteId));
                matches = Process.ExtractTop(new() { Text = term.Strip() }, notes, note => note.Text.Strip(), limit: limit).Select(match => new Match<Note>()
                {
                    Result = match.Value,
                    Score = match.Score
                });
            }
            YamlProcessor.Write(matches);
        }
    }
}