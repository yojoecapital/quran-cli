using System;
using System.IO;

namespace QuranCli
{
    public static class Defaults
    {
        public static readonly string applicationName = "Quran CLI";
        public static readonly string configurationPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "quran-cli");
        public static readonly string databaseFileName = "data.db";
        public static readonly string resourceUrl = "https://yojoecapital.github.io/quran-cli";
        public static readonly string versesFileName = "verses.txt";
        public static readonly string chaptersFileName = "chapters.csv";
        public static readonly string translationsFileName = "translations.txt";
        public static readonly string pagesFileName = "pages.txt";
        public static readonly string databasePath = Path.Join(configurationPath, databaseFileName);
        public static readonly string temporaryPath = Path.Join(Path.GetTempPath(), "quran-cli-resources");
        public static readonly (int min, int max) searchResultLimit = (1, 10);
        public static readonly int maxVersesInMacro = 10;
        public static readonly string addNoteMessage = @"<!-- Type your notes here.                      -->
<!-- You can use a macro like '{<selection>}'   -->
<!-- to expand to a verse selection.            -->
<!-- You can also use a tag like '#<selection>' -->
<!-- Comments like these will be ignored.       -->
<!-- Once finished, save and close the editor.  -->";
    }
}