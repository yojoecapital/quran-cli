using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using QuranCli.Data.Yaml;
using QuranCli.Utilities;

namespace QuranCli.Data.Models
{
    public class Note : IModel, IYamlSerializable
    {
        public int Id { get; set; }
        public string Text { get; set; }

        private static readonly string propertiesString = $"{nameof(Id)}, {nameof(Text)}";

        private static Note PopulateFrom(SqliteDataReader reader)
        {
            return new Note()
            {
                Id = reader.GetInt32(0),
                Text = reader.GetString(1)
            };
        }

        public static void CreateTable()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                CREATE TABLE IF NOT EXISTS {nameof(Note)} (
                    {nameof(Id)} INTEGER PRIMARY KEY AUTOINCREMENT,
                    {nameof(Text)} TEXT NOT NULL
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
                INSERT INTO {nameof(Note)} ({nameof(Text)}) 
                VALUES (@{nameof(Text)});
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue($"@{nameof(Text)}", Text);
            Id = (int)(long)command.ExecuteScalar();
        }

        public void Update()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                UPDATE {nameof(Note)}
                SET {nameof(Text)} = @{nameof(Text)}
                WHERE {nameof(Id)} = @{nameof(Id)};
            ";
            command.Parameters.AddWithValue($"@{nameof(Id)}", Id);
            command.Parameters.AddWithValue($"@{nameof(Text)}", Text);
            command.ExecuteNonQuery();
        }

        public static Note SelectById(int id)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Note)}
                WHERE {nameof(Id)} = @{nameof(id)};
            ";
            command.Parameters.AddWithValue($"@{nameof(id)}", id);
            using var reader = command.ExecuteReader();
            if (reader.Read()) return PopulateFrom(reader);
            throw new Exception($"No note found for ID '{id}'");
        }

        public static IEnumerable<Note> SelectAll()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Note)};
            ";
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return PopulateFrom(reader);
        }

        public static void DeleteById(int id)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                DELETE FROM {nameof(Note)}
                WHERE {nameof(Id)} = @{nameof(id)};
            ";
            command.Parameters.AddWithValue($"@{nameof(id)}", id);
            command.ExecuteNonQuery();
        }

        public IEnumerable<YamlProperty> GetYamlProperties()
        {
            yield return new(nameof(Id), Id);
            yield return new(nameof(Text), MarkdownProcessor.GetColoredStrings(Text));
        }
    }
}