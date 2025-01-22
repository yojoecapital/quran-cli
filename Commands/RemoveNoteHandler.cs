using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Commands
{
    public static class RemoveNoteHandler
    {
        public static void Handle(int id)
        {
            var note = Note.SelectById(id);
            using var translation = ConnectionManager.Connection.BeginTransaction();
            Reference.DeleteByNoteId(note.Id);
            Note.DeleteById(note.Id);
            translation.Commit();
            YamlProcessor.Write(note);
        }
    }
}