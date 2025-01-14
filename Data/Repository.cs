using System;
using Microsoft.Data.Sqlite;

namespace QuranCli.Data
{
    internal partial class Repository : IDisposable
    {
        private readonly SqliteConnection connection;
        private static Repository instance;
        public static Repository Instance
        {
            get
            {
                instance ??= new();
                return instance;
            }
        }

        public static bool IsInitialized => instance != null;

        private Repository()
        {
            connection = new SqliteConnection($"Data Source={Defaults.databasePath}");
            connection.Open();
            Logger.Info($"Connected to database at '{Defaults.databasePath}'.");
        }

        public static void DisposeOfInstance() => instance?.Dispose();

        public void Dispose()
        {
            instance = null;
            connection.Close();
            connection.Dispose();
            Logger.Info($"Closed database connection.");
            GC.SuppressFinalize(this);
        }
    }
}