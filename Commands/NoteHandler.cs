using System;
using QuranCli.Arguments;
using QuranCli.Data;

namespace QuranCli.Commands
{
    internal static class NoteHandler
    {
        public static void Handle(string selectionString, string text, string deleteId)
        {
            if (!AyatSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
        }

        public void OutputAyatNotes(AyatSelection selection)
        {
            var (ayahId1, ayahId2) = selection.GetAyahIds();
            foreach (var ayatNote in Repository.Instance.GetAyatNotesBetween(ayahId1, ayahId2)) 
            {
                Console.WriteLine
            }
        }
    }
}