using System;
using QuranCli.Arguments;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    internal static class NoteHandler
    {
        public static void Handle(AyatSelection selection, string text, AyatSelectionOrGroup selectionOrGroup)
        {
            Console.WriteLine(selection.GetAyahIds() + " " + text + " " + selectionOrGroup?.type);
            if (!string.IsNullOrEmpty(text))
            {
                if (selectionOrGroup == null)
                {
                    LinkCreator.CreateLink(selection, text);
                }
                else if (selectionOrGroup.type == AyatSelectionOrGroup.Type.Group)
                {
                    LinkCreator.CreateLink(selection, text, selectionOrGroup.group.Id);
                }
                else if (selectionOrGroup.type == AyatSelectionOrGroup.Type.Selection)
                {
                    LinkCreator.CreateLink(selection, text, selectionOrGroup.selection);
                }
            }
            else
            {
                var (ayahId1, ayahId2) = selection.GetAyahIds();
                var links = Repository.Instance.GetLinksBetweenAyahIds(ayahId1, ayahId2);
                foreach (var format in NoteFormatter.GetFormattedLines(links))
                {
                    Console.ForegroundColor = format.color;
                    Console.WriteLine(format.text);
                    Console.ResetColor();
                }
            }
        }

        // public void MakeNote(AyatSelection selection, string text)
        // {
        //     var (ayahId1, ayahId2) = selection.GetAyahIds();
        //     var links = Repository.Instance.GetLinksByAyahIds(ayahId1, ayahId2);
        //     if (links.Any(link => link.Group.))
        // }
    }
}