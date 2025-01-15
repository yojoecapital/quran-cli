using System;
using System.IO;

namespace QuranCli
{
    internal static class Defaults
    {
        public static readonly string applicationName = "Quran CLI";
        public static readonly string configurationPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "quran-cli");
        public static readonly string databaseFileName = "data.db";
        public static readonly string resourceUrl = "https://yojoecapital.github.io/quran-cli";
        public static readonly string ayatFileName = "verses.txt";
        public static readonly string surahsFileName = "chapters.csv";
        public static readonly string translationsFileName = "translations.txt";
        public static readonly string databasePath = Path.Join(configurationPath, databaseFileName);
        public static readonly string temporaryPath = Path.Join(Path.GetTempPath(), "quran-cli-resources");
    }
}