using System.IO;
using QuranCli.Data;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    internal static class BuildDatabaseHandler
    {
        public static void Handle()
        {
            File.Delete(Defaults.databasePath);
            var repository = Repository.Instance;
            repository.Build();
        }

    }
}