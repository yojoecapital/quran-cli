using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using QuranCli.Data.Yaml;
using QuranCli.Utilities;

namespace QuranCli.Data.Models
{
    public class Chapter : IModel, IYamlSerializable
    {
        public int Number { get; set; }
        public int Count { get; set; }
        public int Start { get; set; }
        public string Name { get; set; }
        public string Translation { get; set; }
        public string Transliteration { get; set; }
        public int End
        {
            get => Start + Count - 1;
        }

        private static readonly string propertiesString = $"{nameof(Number)}, {nameof(Count)}, {nameof(Start)}, {nameof(Name)}, {nameof(Translation)}, {nameof(Transliteration)}";

        private static Chapter PopulateFrom(SqliteDataReader reader)
        {
            return new Chapter()
            {
                Number = reader.GetInt32(0),
                Count = reader.GetInt32(1),
                Name = reader.GetString(2),
                Translation = reader.GetString(3),
                Transliteration = reader.GetString(4)
            };
        }

        public static void CreateTable()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                CREATE TABLE IF NOT EXISTS {nameof(Chapter)} (
                    {nameof(Number)} INTEGER PRIMARY KEY,
                    {nameof(Count)} INTEGER NOT NULL,
                    {nameof(Start)} INTEGER NOT NULL,
                    {nameof(Name)} VARCHAR NOT NULL,
                    {nameof(Translation)} VARCHAR NOT NULL,
                    {nameof(Transliteration)} VARCHAR NOT NULL
                );
            ";
            command.ExecuteNonQuery();
        }

        public IEnumerable<YamlProperty> GetYamlProperties()
        {
            yield return new(nameof(Number), Number);
            yield return new(nameof(Count), Count);
            yield return new(nameof(Name), Name);
            yield return new(nameof(Translation), Translation);
            yield return new(nameof(Transliteration), Transliteration);
        }

        public void Insert()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                INSERT INTO {nameof(Chapter)} ({propertiesString}) 
                VALUES (@{nameof(Number)}, @{nameof(Count)}, @{nameof(Start)}, @{nameof(Name)}, @{nameof(Translation)}, @{nameof(Transliteration)});
            ";
            command.Parameters.AddWithValue($"@{nameof(Number)}", Number);
            command.Parameters.AddWithValue($"@{nameof(Count)}", Count);
            command.Parameters.AddWithValue($"@{nameof(Start)}", Start);
            command.Parameters.AddWithValue($"@{nameof(Name)}", Name);
            command.Parameters.AddWithValue($"@{nameof(Translation)}", Translation);
            command.Parameters.AddWithValue($"@{nameof(Transliteration)}", Transliteration);
            command.ExecuteNonQuery();
        }

        public static IEnumerable<Chapter> SelectAll()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Chapter)};
            ";
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return PopulateFrom(reader);
        }

        public static Chapter SelectByNumber(int number)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString} 
                FROM {nameof(Chapter)}
                WHERE {nameof(Number)} = @{nameof(number)};
            ";
            command.Parameters.AddWithValue($"@{nameof(number)}", number);
            using var reader = command.ExecuteReader();
            if (reader.Read()) return PopulateFrom(reader);
            throw new Exception($"No chapter found for {number}");
        }
        public static IEnumerable<Chapter> SelectBetweenNumbers(int number1, int number2)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString} 
                FROM {nameof(Chapter)}
                WHERE {nameof(Number)} >= @{nameof(number1)} AND {nameof(Number)} <= @{nameof(number2)};
            ";
            command.Parameters.AddWithValue($"@{nameof(number1)}", number1);
            command.Parameters.AddWithValue($"@{nameof(number2)}", number2);
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return PopulateFrom(reader);
        }

        public static Chapter SelectByTransliteration(string transliteration)
        {
            var chapters = SelectAll();
            var rankings = chapters
                .Select(chapter => new RankedChapter
                {
                    Chapter = chapter,
                    Distance = StringExtensions.ComputeLevenshteinDistance(transliteration, chapter.Transliteration)
                })
                .OrderBy(ranks => ranks.Distance);
            return rankings.First().Chapter;
        }
    }
}