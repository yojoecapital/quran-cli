using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using QuranCli.Data.Yaml;

namespace QuranCli.Data.Models
{
    public class Verse : IModel, IYamlSerializable
    {
        public int Id { get; set; }
        public int Chapter { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
        public string Translation { get; set; }

        private static readonly string propertiesString = $"{nameof(Id)}, {nameof(Chapter)}, {nameof(Number)}, {nameof(Text)}, {nameof(Translation)}";

        private static Verse PopulateFrom(SqliteDataReader reader)
        {
            return new Verse()
            {
                Id = reader.GetInt32(0),
                Chapter = reader.GetInt32(1),
                Number = reader.GetInt32(2),
                Text = reader.GetString(3),
                Translation = reader.GetString(4)
            };
        }

        public static void CreateTable()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                CREATE TABLE IF NOT EXISTS {nameof(Verse)} (
                    {nameof(Id)} INTEGER PRIMARY KEY,
                    {nameof(Chapter)} INTEGER NOT NULL,
                    {nameof(Number)} INTEGER NOT NULL,
                    {nameof(Text)} TEXT NOT NULL,
                    {nameof(Translation)} TEXT,
                    FOREIGN KEY({nameof(Chapter)}) REFERENCES {nameof(Models.Chapter)}({nameof(Models.Chapter.Number)})
                );
            ";
#if DEBUG
            Logger.Info(command.CommandText);
#endif
            command.ExecuteNonQuery();
        }

        public IEnumerable<YamlProperty> GetYamlProperties()
        {
            yield return new(nameof(Chapter), Chapter);
            yield return new(nameof(Number), Number);
            yield return new(nameof(Text), Text);
            yield return new(nameof(Translation), Translation);
        }

        public void Insert()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                INSERT INTO {nameof(Verse)} ({propertiesString}) 
                VALUES (@{nameof(Id)}, @{nameof(Chapter)}, @{nameof(Number)}, @{nameof(Text)}, @{nameof(Translation)});
            ";
            command.Parameters.AddWithValue($"@{nameof(Id)}", Id);
            command.Parameters.AddWithValue($"@{nameof(Chapter)}", Chapter);
            command.Parameters.AddWithValue($"@{nameof(Number)}", Number);
            command.Parameters.AddWithValue($"@{nameof(Text)}", Text);
            command.Parameters.AddWithValue($"@{nameof(Translation)}", Translation);
            command.ExecuteNonQuery();
        }

        public static Verse SelectById(int id)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Verse)}
                WHERE {nameof(Id)} = @{nameof(id)};
            ";
            command.Parameters.AddWithValue($"@{nameof(id)}", id);
            using var reader = command.ExecuteReader();
            if (reader.Read()) return PopulateFrom(reader);
            throw new Exception($"No verse found for ID {id}");
        }

        public static Verse SelectByNumber(int chapterNumber, int verseNumber)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Verse)}
                WHERE {nameof(Chapter)} = @{nameof(chapterNumber)} AND {nameof(Number)} = @{nameof(verseNumber)};
            ";
            command.Parameters.AddWithValue($"@{nameof(chapterNumber)}", chapterNumber);
            command.Parameters.AddWithValue($"@{nameof(verseNumber)}", verseNumber);
            using var reader = command.ExecuteReader();
            if (reader.Read()) return PopulateFrom(reader);
            throw new Exception($"No verse found for {chapterNumber}:{verseNumber}");
        }

        public static IEnumerable<Verse> SelectByNumber(int chapterNumber)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Verse)}
                WHERE {nameof(Chapter)} = @{nameof(chapterNumber)};
            ";
            command.Parameters.AddWithValue($"@{nameof(chapterNumber)}", chapterNumber);
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return PopulateFrom(reader);
        }

        public static IEnumerable<Verse> SelectBetweenIds(int verseId1, int verseId2)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Verse)}
                WHERE {nameof(Id)} >= @{nameof(verseId1)} AND {nameof(Id)} <= @{nameof(verseId2)};
            ";
            command.Parameters.AddWithValue($"@{nameof(verseId1)}", verseId1);
            command.Parameters.AddWithValue($"@{nameof(verseId2)}", verseId2);
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return PopulateFrom(reader);
        }

        public static IEnumerable<Verse> SelectAll()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Verse)};
            ";
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return PopulateFrom(reader);
        }


        public static string GetDisplayName(int id)
        {
            var verse = SelectById(id);
            var chapter = Models.Chapter.SelectByNumber(verse.Chapter);
            return $"{chapter.Transliteration} {verse.Number}";
        }

        public static string GetDisplayName(int id1, int id2)
        {
            if (id1 == id2) return GetDisplayName(id1);
            var verse1 = SelectById(id1);
            var chapter1 = Models.Chapter.SelectByNumber(verse1.Chapter);
            var verse2 = SelectById(id2);
            var chapter2 = Models.Chapter.SelectByNumber(verse2.Chapter);
            if (chapter1.Number == chapter2.Number)
            {
                if (verse1.Id == chapter1.Start && verse2.Id == chapter2.End) return chapter1.Transliteration;
                return $"{chapter1.Transliteration} {verse1.Number} → {verse2.Number}";
            }
            if (verse1.Id == chapter1.Start && verse2.Id == chapter2.End) return $"{chapter1.Transliteration} → {chapter2.Transliteration}";
            return $"{chapter1.Transliteration} {verse1.Number} → {chapter2.Transliteration} {verse2.Number}";
        }
    }
}