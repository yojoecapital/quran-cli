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
            foreach (var id in ids)
            {
                var note = Note.SelectById(id);
                Reference.DeleteByNoteId(note.Id);
                Note.DeleteById(note.Id);
                YamlProcessor.Write(note);
            }
            translation.Commit();
        }
    }
}