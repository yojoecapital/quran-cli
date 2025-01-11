using System;
using System.IO;

namespace QuranCli
{
    internal static class Defaults
    {
        public static readonly string applicationName = "Quran CLI";
        public static readonly string configurationPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "quran");
        public static readonly string databasePath = Path.Join(configurationPath, "data.db");
        public static readonly string temporaryPath = Path.Join(configurationPath, "tmp");
        public static readonly string resourceUrl = "https://yojoecapital.github.io/quran-cli";
        public static readonly string versesFileName = "verses.txt";
        public static readonly string surahsFileName = "surahs.csv";
        public static readonly string translationsFileName = "translations.txt";
    }
}