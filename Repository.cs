using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace QuranCli
{
    internal partial class Repository : IDisposable
    {
        private readonly string databasePath;
        private readonly SqliteConnection connection;

        public Repository()
        {
            var configDirectory = Path.Combine(Defaults.configurationPath, "quran");
            Directory.CreateDirectory(configDirectory);
            databasePath = Path.Combine(configDirectory, "quran.db");
            var shouldCreateDatabase = !File.Exists(databasePath);
            connection = new SqliteConnection($"Data Source={databasePath}");
            connection.Open();
            if (shouldCreateDatabase)
            {
                CreateDatabase();
                // SeedDatabase();
            }
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateDatabase()
        {
            // Create Ayah table
            var createAyatTableCommand = connection.CreateCommand();
            createAyatTableCommand.CommandText = @"
                CREATE TABLE Ayah (
                    ayahId INTEGER PRIMARY KEY AUTOINCREMENT,
                    surahId INTEGER,
                    ayahNumber INTEGER,
                    surahNumber INTEGER,
                    text TEXT,
                    translation TEXT,
                    note TEXT
                );
            ";
            createAyatTableCommand.ExecuteNonQuery();

            // Create Surah table
            var createSurahTableCommand = connection.CreateCommand();
            createSurahTableCommand.CommandText = @"
                CREATE TABLE Surah (
                    surahId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ayahCount INTEGER,
                    name TEXT,
                    englishName TEXT,
                    transliterationName TEXT,
                    note TEXT
                );
            ";
            createSurahTableCommand.ExecuteNonQuery();

            // Create AyahLink table
            var createAyahLinkTableCommand = connection.CreateCommand();
            createAyahLinkTableCommand.CommandText = @"
                CREATE TABLE AyahLink (
                    ayahLinkId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ayahId1 INTEGER,
                    ayahId2 INTEGER,
                    note TEXT
                );
            ";
            createAyahLinkTableCommand.ExecuteNonQuery();

            // Create Group table
            var createGroupTableCommand = connection.CreateCommand();
            createGroupTableCommand.CommandText = @"
                CREATE TABLE Group (
                    groupId INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    note TEXT
                );
            ";
            createGroupTableCommand.ExecuteNonQuery();

            // Create GroupLink table
            var createGroupLinkTableCommand = connection.CreateCommand();
            createGroupLinkTableCommand.CommandText = @"
                CREATE TABLE GroupLink (
                    groupLinkId INTEGER PRIMARY KEY AUTOINCREMENT,
                    groupId INTEGER,
                    ayahId INTEGER,
                    note TEXT
                );
            ";
            createGroupLinkTableCommand.ExecuteNonQuery();
        }
    }
}