using Microsoft.Data.Sqlite;

namespace QuranCli.Utilities
{
    internal static class TableCreator
    {
        public static void CreateTables(this SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE Surah (
                    id INTEGER PRIMARY KEY,
                    ayahCount INTEGER NOT NULL,
                    startAyahId INTEGER NOT NULL,
                    name TEXT NOT NULL,
                    englishName TEXT NOT NULL,
                    transliterationName TEXT NOT NULL
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE Ayah (
                    id INTEGER PRIMARY KEY,
                    surahId INTEGER NOT NULL,
                    ayahNumber INTEGER NOT NULL,
                    verse TEXT NOT NULL,
                    translation TEXT,
                    FOREIGN KEY (surahId) REFERENCES Surah(id)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE INDEX idx_ayah_surahId_ayahNumber ON Ayah(surahId, ayahNumber);
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE SurahNote (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    surahId INTEGER NOT NULL,
                    text TEXT NOT NULL,
                    FOREIGN KEY (surahId) REFERENCES Surah(id)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE INDEX idx_surahNote_surahId ON SurahNote(surahId);
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE [Group] (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    text TEXT
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE INDEX idx_group_name ON [Group](name);
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE Link (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    groupId INTEGER NOT NULL,
                    ayahId1 INTEGER NOT NULL,
                    ayahId2 INTEGER NOT NULL,
                    [from] INTEGER,
                    [to] INTEGER,
                    FOREIGN KEY (groupId) REFERENCES [Group](id),
                    FOREIGN KEY (ayahId1) REFERENCES Ayah(id),
                    FOREIGN KEY (ayahId2) REFERENCES Ayah(id)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE INDEX idx_link_ayahId1 ON Link(ayahId1);
                CREATE INDEX idx_link_ayahId2 ON Link(ayahId2);
            ";
            command.ExecuteNonQuery();
        }
    }
}
