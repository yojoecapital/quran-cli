using System;
using Microsoft.Data.Sqlite;

namespace QuranCli.Data.Models
{
    public class Page : IModel
    {
        public int Number { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        private static readonly string propertiesString = $"{nameof(Number)}, {nameof(Start)}, {nameof(End)}";

        private static Page PopulateFrom(SqliteDataReader reader)
        {
            return new Page()
            {
                Number = reader.GetInt32(0),
                Start = reader.GetInt32(1),
                End = reader.GetInt32(2)
            };
        }

        public static void CreateTable()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                CREATE TABLE IF NOT EXISTS {nameof(Page)} (
                    {nameof(Number)} INTEGER PRIMARY KEY AUTOINCREMENT,
                    {nameof(Start)} INTEGER NOT NULL,
                    {nameof(End)} INTEGER NOT NULL,
                    FOREIGN KEY({nameof(Start)}) REFERENCES {nameof(Verse)}({nameof(Verse.Id)}),
                    FOREIGN KEY({nameof(End)}) REFERENCES {nameof(Verse)}({nameof(Verse.Id)})
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
                VALUES (@{nameof(Number)}, @{nameof(Start)}, @{nameof(End)});
            ";
            command.Parameters.AddWithValue($"@{nameof(Number)}", Number);
            command.Parameters.AddWithValue($"@{nameof(Start)}", Start);
            command.Parameters.AddWithValue($"@{nameof(End)}", End);
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