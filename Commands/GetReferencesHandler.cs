using System.Linq;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Commands
{
    public static class GetReferencesHandler
    {
        public static void Handle(int id)
        {
            YamlProcessor.Write(Reference.SelectByNoteId(id).Select(GetDisplayName));
        }

        private static string GetDisplayName(Reference reference)
        {
            return Verse.GetDisplayName(reference.VerseId1, reference.VerseId2) + " " + (reference.IsTag ? "Tag" : "Macro");
        }
    }
}