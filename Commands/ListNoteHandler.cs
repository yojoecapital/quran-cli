using System;
using System.Collections.Generic;
using QuranCli.Arguments;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Commands
{
    public static class ListNoteHandler
    {
        public static void Handle(string selectionString, int? id)
        {
            if (selectionString == null)
            {
                if (id.HasValue)
                {
                    var note = Note.SelectById(id.Value);
                    YamlProcessor.Write(note);
                    return;
                }
                foreach (var note in Note.SelectAll()) YamlProcessor.Write(note);
                return;
            }
            if (!VerseSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            var (verseId1, verseId2) = selection.GetVerseIds();
            var references = Reference.SelectBetween(verseId1, verseId2);
            var set = new HashSet<int>();
            if (id.HasValue)
            {
                foreach (var reference in references)
                {
                    if (reference.NoteId == id)
                    {
                        var note = Note.SelectById(reference.NoteId);
                        YamlProcessor.Write(note);
                        return;
                    }
                }
                throw new Exception($"No note found for ID {id} in this selection");
            }
            else
            {
                foreach (var reference in references)
                {
                    if (set.Contains(reference.NoteId)) continue;
                    set.Add(reference.NoteId);
                    var note = Note.SelectById(reference.NoteId);
                    YamlProcessor.Write(note);
                }
            }
        }
    }
}