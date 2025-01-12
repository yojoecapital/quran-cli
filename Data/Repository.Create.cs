using Microsoft.Data.Sqlite;
using QuranCli.Data.Models;
using QuranCli.Utilities;
using System;
using System.IO;
using Dapper;

namespace QuranCli.Data
{
    internal partial class Repository : IDisposable
    {
        public readonly SqliteConnection connection;

        public Repository()
        {
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
            const string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name='Settings';";
            var result = connection.QueryFirstOrDefault<string>(sql);
            return !string.IsNullOrEmpty(result);
        }

        public void Create(Ayah ayah)
        {
            const string sql = @"
                INSERT INTO Ayah (id, surahId, ayahNumber, verse, translation) 
                VALUES (@Id, @SurahId, @AyahNumber, @Verse, @Translation);
            ";
            connection.Execute(sql, ayah);
        }

        public void Create(Surah surah)
        {
            const string sql = @"
                INSERT INTO Surah (id, ayahCount, startAyahId, name, englishName, transliterationName) 
                VALUES (@Id, @AyahCount, @StartAyahId, @Name, @EnglishName, @TransliterationName);
            ";
            connection.Execute(sql, surah);
        }

        public void Create(SurahNote surahNote)
        {
            const string sql = @"
                INSERT INTO SurahNote (surahId, text) 
                VALUES (@SurahId, @Text);
            ";
            connection.Execute(sql, surahNote);
        }

        public void Create(Group node)
        {
            const string sql = @"
                INSERT INTO [Group] (name, text) 
                VALUES (@Name, @Text);
            ";
            connection.Execute(sql, node);
        }

        public void Create(Link link)
        {
            const string sql = @"
                INSERT INTO Link (nodeId, ayahId1, ayahId2, [from], [to]) 
                VALUES (@NodeId, @AyahId1, @AyahId2, @From, @To);
            ";
            connection.Execute(sql, link);
        }
    }
}