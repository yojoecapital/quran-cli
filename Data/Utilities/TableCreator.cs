using Microsoft.Data.Sqlite;

namespace QuranCli.Data.Utilities
{
    internal static class TableCreator
    {
        public static void CreateTables(this SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE Ayah (
                    id INTEGER PRIMARY KEY,
                    surahId INTEGER NOT NULL,
                    ayahNumber INTEGER NOT NULL,
                    verse TEXT NOT NULL,
                    translation TEXT,
                    FOREIGN KEY (surahId) REFERENCES Surah(id)
                );
                CREATE INDEX idx_ayah_surahId_ayahNumber ON Ayah(surahId, ayahNumber);
            ";
            command.ExecuteNonQuery();
            command = connection.CreateCommand();
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
            command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE SurahNote (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    surahId INTEGER NOT NULL,
                    text TEXT NOT NULL,
                    FOREIGN KEY (surahId) REFERENCES Surah(id)
                );
                CREATE INDEX idx_surahNote_surahId ON SurahNote(surahId);
            ";
            command.ExecuteNonQuery();
            command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE Node (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    text TEXT NOT NULL
                );
            ";
            command.ExecuteNonQuery();
            command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE Link (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    nodeId INTEGER NOT NULL,
                    ayahId INTEGER NOT NULL,
                    FOREIGN KEY (nodeId) REFERENCES Node(id),
                    FOREIGN KEY (ayahId) REFERENCES Ayah(id)
                );
                CREATE INDEX idx_link_ayahId ON Link(ayahId);
            ";
            command.ExecuteNonQuery();
        }
    }
}