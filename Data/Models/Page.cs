using System;
using Microsoft.Data.Sqlite;

namespace QuranCli.Data.Models
{
    public class Page : IModel
    {
        public int Number { get; set; }
        public int VerseId { get; set; }

        private static readonly string propertiesString = $"{nameof(Number)}, {nameof(VerseId)}";

        private static Page PopulateFrom(SqliteDataReader reader)
        {
            return new Page()
            {
                Number = reader.GetInt32(0),
                VerseId = reader.GetInt32(1)
            };
        }

        public static void CreateTable()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                CREATE TABLE IF NOT EXISTS {nameof(Page)} (
                    {nameof(Number)} INTEGER PRIMARY KEY AUTOINCREMENT,
                    {nameof(VerseId)} INTEGER NOT NULL,
                    FOREIGN KEY({nameof(VerseId)}) REFERENCES {nameof(Verse)}({nameof(Verse.Id)})
                );
            ";
#if DEBUG
            Logger.Info(command.CommandText);
#endif
            command.ExecuteNonQuery();
        }

        public void Insert()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                INSERT OR IGNORE INTO {nameof(Page)} ({propertiesString}) 
                VALUES (@{nameof(Number)}, @{nameof(VerseId)});
            ";
            command.Parameters.AddWithValue($"@{nameof(Number)}", Number);
            command.Parameters.AddWithValue($"@{nameof(VerseId)}", VerseId);
            command.ExecuteNonQuery();
        }

        public static Page SelectByNumber(int number)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Page)}
                WHERE {nameof(Number)} = @{nameof(number)};
            ";
            command.Parameters.AddWithValue($"@{nameof(number)}", number);
            using var reader = command.ExecuteReader();
            if (reader.Read()) return PopulateFrom(reader);
            throw new Exception($"No page found for 'p{number}'");
        }
    }
}