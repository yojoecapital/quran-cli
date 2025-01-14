using System;
using System.Collections.Generic;
using System.Linq;
using QuranCli.Arguments;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    internal static class NoteHandler
    {
        public static void Handle(string selectionString, string note)
        {
            if (!AyatSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            Logger.Info(selection.GetLog());
            if (Console.IsInputRedirected) note = Console.In.ReadToEnd();
            var (ayahId1, ayahId2) = selection.GetAyahIds();
            if (note != null)
            {
                note = note.ExpandSelectionAnnotations();
                if (selection.isSurahSelection)
                {
                    Repository.Instance.CreateOrEdit(new SurahNote()
                    {
                        SurahId1 = selection.surahId1,
                        SurahId2 = selection.surahId2,
                        Note = note
                    });
                }
                else
                {
                    Repository.Instance.CreateOrEdit(new AyatNote()
                    {
                        AyahId1 = ayahId1,
                        AyahId2 = ayahId2,
                        Note = note
                    });
                }
            }
            var notes = NoteCollection.Join(
                Repository.Instance.GetAyatNotesBetween(ayahId1, ayahId2),
                Repository.Instance.GetSurahNotesBetween(selection.surahId1, selection.surahId2)
            );
            var links = Repository.Instance.GetDirectLinksBetween(ayahId1, ayahId2);
            var groupingLinks = Repository.Instance.GetGroupingLinksBetween(ayahId1, ayahId2);
            var groupingLookup = new Dictionary<int, Grouping>();
            foreach (var link in groupingLinks) groupingLookup[link.GroupId] = null;
            foreach (var id in groupingLookup.Keys) groupingLookup[id] = Repository.Instance.GetGroupingById(id);
            foreach (var grouping in groupingLookup.Values) Repository.Instance.PopulateGrouping(grouping);
            var groupings = groupingLookup.Values;
            var collection = new NoteCollection()
            {
                Notes = notes,
                Links = links,
                Groupings = groupings
            };
            Logger.Message(collection.ToYaml());
            Repository.DisposeOfInstance();
        }
    }
}