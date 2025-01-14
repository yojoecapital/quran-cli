using System;
using System.Collections.Generic;
using QuranCli.Arguments;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Commands
{
    internal static class NoteHandler
    {
        public static void Handle(string selectionString, string text, int? deleteId)
        {
            if (!AyatSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            var shouldCreateOrEdit = text != null;
            var shouldDelete = deleteId == null || deleteId != -1;
            var shouldOutput = !shouldCreateOrEdit && !shouldDelete;
            var (ayahId1, ayahId2) = selection.GetAyahIds();
            if (shouldCreateOrEdit)
            {
                Repository.Instance.CreateOrEdit(new AyatNote()
                {
                    AyahId1 = ayahId1,
                    AyahId2 = ayahId2,
                    Note = text.Trim()
                });
            }
            if (shouldDelete)
            {
                if (deleteId.HasValue) Repository.Instance.DeleteAyatNote(deleteId.Value, ayahId1, ayahId2);
                else Repository.Instance.DeleteAyatNotesBetween(ayahId1, ayahId2);
            }
            if (shouldOutput) Console.WriteLine(YamlSerializable.ToYaml(Repository.Instance.GetAyatNotesBetween(ayahId1, ayahId2)));
            Repository.Instance.Dispose();
        }
    }
}