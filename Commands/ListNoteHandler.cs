using System;
using System.Collections.Generic;
using System.Linq;
using QuranCli.Arguments;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Commands
{
    public static class ListNoteHandler
    {
        public static void Handle(string selectionString, ListByOption listByOption)
        {
            if (selectionString == null)
            {
                if (listByOption == ListByOption.Both)
                {
                    YamlProcessor.Write(Note.SelectAll());
                    return;
                }
                // if there is a --by option, then filter with tags
                selectionString = "all";
            }
            if (!VerseSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            var (verseId1, verseId2) = selection.GetVerseIds();
            IEnumerable<Reference> references;
            if (listByOption == ListByOption.Tag) references = Reference.SelectBetween(verseId1, verseId2, true);
            else if (listByOption == ListByOption.Macro) references = Reference.SelectBetween(verseId1, verseId2, false);
            else references = Reference.SelectBetween(verseId1, verseId2);
            YamlProcessor.Write(references.DistinctBy(reference => reference.NoteId).Select(reference => Note.SelectById(reference.NoteId)));
        }
    }
}