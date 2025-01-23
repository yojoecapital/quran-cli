using System;
using System.IO;
using System.Linq;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    public static class EditNoteHandler
    {
        public static void Handle(int id, string text, bool force)
        {
            var note = Note.SelectById(id);
            if (Console.IsInputRedirected)
            {
                text = Console.In.ReadToEnd();
            }
            else if (string.IsNullOrEmpty(text))
            {
                text = EditorHelper.OpenEditorAndReadInput(note.Text);
            }
            note.Text = MarkdownProcessor.FilterOutComments(text);
            var references = MarkdownProcessor.GetReferences(text).ToArray();
            if (!force && references.Length == 0) throw new Exception("To create a note with no references, use '--force'");
            Logger.Info($"Found {references.Length} reference(s).");
            using var translation = ConnectionManager.Connection.BeginTransaction();
            Reference.DeleteByNoteId(note.Id);
            note.Update();
            foreach (var reference in references)
            {
                reference.NoteId = note.Id;
                reference.Insert();
            }
            translation.Commit();
            YamlProcessor.Write(note);
        }
    }
}