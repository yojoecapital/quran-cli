using System.IO;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    public static class BuildDatabaseHandler
    {
        public static void Handle()
        {
            File.Delete(Defaults.databasePath);
            DatabaseBuilder.Build();
        }

    }
}