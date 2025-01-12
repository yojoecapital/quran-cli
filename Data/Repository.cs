using System;
using Microsoft.Data.Sqlite;

namespace QuranCli.Data
{
    internal partial class Repository
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


        private Repository()
        {
            connection = new SqliteConnection($"Data Source={Defaults.databasePath}");
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}