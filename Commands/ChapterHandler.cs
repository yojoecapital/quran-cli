using System;
using QuranCli.Arguments;
using QuranCli.Data.Yaml;

namespace QuranCli.Commands
{
    public static class ChapterHandler
    {
        public static void Handle(string selectionString)
        {
            if (!ChapterSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            var chapters = selection.GetChapters();
            YamlProcessor.Write(chapters);
        }
    }
}