using System;
using System.Collections.Generic;
using System.Linq;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    internal static class NoteFormatter
    {
        public static IEnumerable<(string text, ConsoleColor color)> GetFormattedLines(IEnumerable<Link> links, int headerDepth = 1)
        {
            links = links.Where(link => link.Group != null);
            var directLinks = links.Where(link => link.Group.Name == null);
            var groupLinks = links.Where(link => link.Group.Name != null);
            yield return GetHeaderFormat(headerDepth, "Links");
            foreach (var link in links)
            {
                var group = link.Group;
                yield return GetListFormat(1, group.Text);
                // FIXME
                foreach (var nestedLink in group.Links) yield return GetListFormat(2, $"{nestedLink.NestedLinkAyahId1} to {nestedLink.NestedLinkAyahId2}");
            }
            yield return GetHeaderFormat(headerDepth, "Groups");
            foreach (var link in groupLinks)
            {
                var group = link.Group;
                yield return GetHeaderFormat(headerDepth + 1, group.Name);
                yield return (group.Text, ConsoleColor.White);
                // FIXME
                foreach (var nestedLink in group.Links) yield return GetListFormat(1, $"{nestedLink.NestedLinkAyahId1} to {nestedLink.NestedLinkAyahId2}");
            }
        }

        public static (string, ConsoleColor) GetHeaderFormat(int depth, string text)
        {
            return (new string('#', depth) + ' ' + text, ConsoleColor.Yellow);
        }

        public static (string, ConsoleColor) GetListFormat(int depth, string text)
        {
            var color = depth == 1 ? ConsoleColor.Blue : ConsoleColor.Cyan;
            return ('-' + new string(' ', depth * 2) + ' ' + text, color);
        }
    }
}