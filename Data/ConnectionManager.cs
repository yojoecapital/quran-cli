using Microsoft.Data.Sqlite;

namespace QuranCli.Data
{
    public static class ConnectionManager
    {
        private static SqliteConnection connection;
        public static SqliteConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new SqliteConnection($"Data Source={Defaults.databasePath}");
                    connection.Open();
                    Logger.Info($"Connected to database at '{Defaults.databasePath}'.");
                }
                return connection;
            }
        }

        public static void Close()
        {
            if (connection == null) return;
            connection.Close();
            connection.Dispose();
            Logger.Info($"Closed database connection.");
        }
    }
}