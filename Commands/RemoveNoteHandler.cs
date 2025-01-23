using System;
using System.Linq;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Commands
{
    public static class RemoveNoteHandler
    {
        public static void Handle(int[] ids)
        {
            using var translation = ConnectionManager.Connection.BeginTransaction();
            var notes = ids.Select(Remove).ToArray();
            translation.Commit();
            YamlProcessor.Write(notes);
        }

        public static Note Remove(int id)
        {
            var note = Note.SelectById(id);
            Reference.DeleteByNoteId(note.Id);
            Note.DeleteById(note.Id);
            return note;
        }
    }
}