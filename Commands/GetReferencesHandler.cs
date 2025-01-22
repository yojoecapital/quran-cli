using System;
using System.IO;
using System.Linq;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Commands
{
    public static class GetReferencesHandler
    {
        public static void Handle(int id)
        {
            YamlProcessor.Write(Reference.SelectByNoteId(id).Select(reference => Verse.GetDisplayName(reference.VerseId1, reference.VerseId2)));
        }
    }
}