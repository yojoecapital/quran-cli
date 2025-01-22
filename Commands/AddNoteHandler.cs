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
        public static void Handle(string path)
        {
            string text;
            if (Console.IsInputRedirected)
            {
                text = Console.In.ReadToEnd();
            }
            else if (!string.IsNullOrEmpty(path))
            {
                text = File.ReadAllText(path);
            }
            else
            {
                text = EditorHelper.OpenEditorAndReadInput(@"<!-- you can type your notes here -->
<!-- use '{<selection>}' to expand to a verse selection -->
<!-- use '#<selection>' to give the note a tag -->
<!-- once finished, save and close the editor -->"
                );
            }
            text = MarkdownProcessor.FilterOutComments(text);
            var references = MarkdownProcessor.GetReferences(text).ToArray();
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
            Logger.Info($"Created new note with ID {note.Id}.");
            YamlProcessor.Write(note);
        }

        private static void Write(Note note)
        {
            foreach (var part in MarkdownProcessor.GetColoredStrings(note.Text))
            {
                Console.ForegroundColor = part.color ?? ConsoleColor.White;
                Console.Write(part.text);
                Console.ResetColor();
            }
        }
    }
}