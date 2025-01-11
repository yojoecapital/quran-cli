using Microsoft.Data.Sqlite;
using QuranCli.Data.Models;
using QuranCli.Data.Utilities;
using System;
using System.IO;
using Dapper;

namespace QuranCli.Data
{
    internal class Repository : IDisposable
    {
        private readonly SqliteConnection connection;

        public Repository()
        {
            Directory.CreateDirectory(Defaults.configurationPath);
            var shouldInitialize = !File.Exists(Defaults.databasePath);
            connection = new SqliteConnection($"Data Source={Defaults.databasePath}");
            connection.Open();
            if (shouldInitialize) Initialize();
            else if (!HasSettingsTable()) throw new Exception("The database was not initialized correctly.");
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            connection.CreateTables();
            this.Seed();
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Settings (
                    id INTEGER PRIMARY KEY,
                    value TEXT NOT NULL
                );
            ";
            command.ExecuteNonQuery();
        }

        public bool HasSettingsTable()
        {
            var sql = "SELECT name FROM sqlite_master WHERE type='table' AND name='Settings';";
            var result = connection.QueryFirstOrDefault<string>(sql);
            return !string.IsNullOrEmpty(result);
        }

        public void Create(Ayah ayah)
        {
            var sql = @"
                INSERT INTO Ayah (id, surahId, ayahNumber, verse, translation) 
                VALUES (@Id, @SurahId, @AyahNumber, @Verse, @Translation);
            ";
            connection.Execute(sql, ayah);
        }

        public void Create(Surah surah)
        {
            var sql = @"
                INSERT INTO Surah (id, ayahCount, startAyahId, name, englishName, transliterationName) 
                VALUES (@Id, @AyahCount, @StartAyahId, @Name, @EnglishName, @TransliterationName);
            ";
            connection.Execute(sql, surah);
        }

        public void Create(SurahNote surahNote)
        {
            var sql = @"
                INSERT INTO SurahNote (surahId, text) 
                VALUES (@SurahId, @Text);
            ";
            connection.Execute(sql, surahNote);
        }

        public void Create(Node node)
        {
            var sql = @"
                INSERT INTO Node (name, text) 
                VALUES (@Name, @Text);
            ";
            connection.Execute(sql, node);
        }

        public void Create(Link link)
        {
            var sql = @"
                INSERT INTO Link (nodeId, ayahId) 
                VALUES (@NodeId, @AyahId);
            ";
            connection.Execute(sql, link);
        }
    }
}