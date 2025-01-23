using System;
using System.IO;
using System.Linq;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    public static class AddNoteHandler
    {
        public static void Handle(string text, bool force)
        {
            if (Console.IsInputRedirected)
            {
                text = Console.In.ReadToEnd();
            }
            else if (string.IsNullOrEmpty(text))
            {
                text = EditorHelper.OpenEditorAndReadInput(Defaults.addNoteMessage);
            }
            text = MarkdownProcessor.FilterOutComments(text);
            if (string.IsNullOrWhiteSpace(text)) throw new Exception("Note is empty");
            var references = MarkdownProcessor.GetReferences(text).ToArray();
            if (!force && references.Length == 0) throw new Exception("To create a note with no references, use '--force'");
            Logger.Info($"Found {references.Length} reference(s).");
            using var translation = ConnectionManager.Connection.BeginTransaction();
            var note = new Note() { Text = text };
            note.Insert();
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