using System;
using System.IO;

namespace QuranCli
{
    internal static class Defaults
    {
        public static readonly string applicationName = "Quran CLI";
        public static readonly string configurationPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "goop");
    }
}