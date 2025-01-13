using Microsoft.Data.Sqlite;

namespace QuranCli.Utilities
{
    internal static class TableCreator
    {
        public static void CreateTables(this SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Create the Surah table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Surah (
                    id INTEGER PRIMARY KEY,
                    ayahCount INTEGER NOT NULL,
                    startAyahId INTEGER NOT NULL,
                    name VARCHAR NOT NULL,
                    englishName VARCHAR NOT NULL,
                    transliterationName VARCHAR NOT NULL
                );
            ";
            command.ExecuteNonQuery();

            // Create the Ayah table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Ayah (
                    id INTEGER PRIMARY KEY,
                    surahId INTEGER NOT NULL,
                    ayahNumber INTEGER NOT NULL,
                    verse TEXT NOT NULL,
                    translation TEXT,
                    FOREIGN KEY (surahId) REFERENCES Surah(id)
                );
            ";
            command.ExecuteNonQuery();

            // Create the Ayah FTS table for fuzzy search
            command.CommandText = @"
                CREATE VIRTUAL TABLE IF NOT EXISTS AyahFts USING fts5(
                    verse, 
                    translation, 
                    content='Ayah',
                    content_rowid='id'
                );
            ";
            command.ExecuteNonQuery();

            // Index for Ayah table
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_ayah_surahId_ayahNumber ON Ayah(surahId, ayahNumber);
            ";
            command.ExecuteNonQuery();

            // Create the SurahNote table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS SurahNote (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    surahId INTEGER NOT NULL,
                    text TEXT NOT NULL,
                    FOREIGN KEY (surahId) REFERENCES Surah(id)
                );
            ";
            command.ExecuteNonQuery();

            // Index for SurahNote table
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_surahNote_surahId ON SurahNote(surahId);
            ";
            command.ExecuteNonQuery();

            // Create the Group table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS [Group] (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    text TEXT
                );
            ";
            command.ExecuteNonQuery();

            // Index for Group table
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_group_name ON [Group](name);
            ";
            command.ExecuteNonQuery();

            // Create the Link table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Link (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    groupId INTEGER NOT NULL,
                    ayahId1 INTEGER NOT NULL,
                    ayahId2 INTEGER NOT NULL,
                    FOREIGN KEY (groupId) REFERENCES [Group](id),
                    FOREIGN KEY (ayahId1) REFERENCES Ayah(id),
                    FOREIGN KEY (ayahId2) REFERENCES Ayah(id),
                    UNIQUE (ayahId1, ayahId2, groupId)
                );
            ";
            command.ExecuteNonQuery();

            // Indexes for Link table
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_link_groupId1 ON Link(groupId);
                CREATE INDEX IF NOT EXISTS idx_link_ayahId1 ON Link(ayahId1);
                CREATE INDEX IF NOT EXISTS idx_link_ayahId2 ON Link(ayahId2);
            ";
            command.ExecuteNonQuery();
        }
    }
}
