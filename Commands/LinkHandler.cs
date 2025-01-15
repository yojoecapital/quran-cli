using System;
using System.Collections.Generic;
using QuranCli.Arguments;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    internal static class LinkHandler
    {
        public static void Handle(string selectionString1, string selectionString2, string note)
        {
            if (!AyatSelection.TryParse(selectionString1, out var selection1)) throw new Exception("Could not parse selection");
            Logger.Info(selection1.GetLog());
            var grouping = Repository.Instance.GetGroupingByName(selectionString2);
            if (Console.IsInputRedirected) note = Console.In.ReadToEnd();
            var (ayahId1, ayahId2) = selection1.GetAyahIds();
            if (grouping == null)
            {
                if (!AyatSelection.TryParse(selectionString1, out var selection2)) throw new Exception("No group exists and could not parse selection");
                Logger.Info(selection2.GetLog());
                var (ayahId3, ayahId4) = selection2.GetAyahIds();
                HandleDirectLink(ayahId1, ayahId2, ayahId3, ayahId4, note);
            }
            else HandleGroupingLink(ayahId1, ayahId2, grouping, note);
            Repository.DisposeOfInstance();
        }

        private static void HandleDirectLink(int ayahId1, int ayahId2, int ayahId3, int ayahId4, string note)
        {
            if (ayahId3 < ayahId1)
            {
                (ayahId1, ayahId3) = (ayahId3, ayahId1);
                (ayahId2, ayahId4) = (ayahId4, ayahId2);
            }
            var link = new DirectLink()
            {
                AyahId1 = ayahId1,
                AyahId2 = ayahId2,
                AyahId3 = ayahId3,
                AyahId4 = ayahId4,
            };
            if (note != null || Repository.Instance.GetDirectLink(ayahId1, ayahId2, ayahId3, ayahId4) == null)
            {
                link.Note = note.ExpandSelectionAnnotations();
                Repository.Instance.CreateOrEdit(link);
            }
            var links = Repository.Instance.GetDirectLinksBetween(ayahId1, ayahId2, ayahId3, ayahId4);
            var links1 = Repository.Instance.GetGroupingLinksBetween(ayahId1, ayahId2);
            var links2 = Repository.Instance.GetGroupingLinksBetween(ayahId3, ayahId4);
            var groupingLookup = new Dictionary<int, Grouping>();
            foreach (var link1 in links1) groupingLookup[link1.GroupId] = null;
            foreach (var link2 in links2) groupingLookup[link2.GroupId] = null;
            foreach (var id in groupingLookup.Keys) groupingLookup[id] = Repository.Instance.GetGroupingById(id);
            foreach (var grouping in groupingLookup.Values) Repository.Instance.PopulateGrouping(grouping);
            var groupings = groupingLookup.Values;
            var collection = new NoteCollection()
            {
                Links = links,
                Groupings = groupings
            };
            Logger.Message(collection.ToYaml());
        }

        private static void HandleGroupingLink(int ayahId1, int ayahId2, Grouping grouping, string note)
        {
            Repository.Instance.Create(new GroupingLink()
            {
                AyahId1 = ayahId1,
                AyahId2 = ayahId2,
                GroupId = grouping.Id
            });
            if (note != null)
            {
                Repository.Instance.Edit(new Grouping()
                {
                    Name = grouping.Name,
                    Note = note.ExpandSelectionAnnotations()
                });
            }
            Repository.Instance.PopulateGrouping(grouping);
            Logger.Message(grouping.ToYaml());
        }
    }
}