using Microsoft.Data.Sqlite;

namespace QuranCli.Utilities
{
    internal static class TableCreator
    {
        public static void CreateTables(this SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Surah (
                    Id INTEGER PRIMARY KEY,
                    AyahCount INTEGER NOT NULL,
                    StartAyahId INTEGER NOT NULL,
                    Name VARCHAR NOT NULL,
                    EnglishName VARCHAR NOT NULL,
                    TransliterationName VARCHAR NOT NULL
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Ayah (
                    Id INTEGER PRIMARY KEY,
                    SurahId INTEGER NOT NULL,
                    AyahNumber INTEGER NOT NULL,
                    Verse TEXT NOT NULL,
                    Translation TEXT,
                    FOREIGN KEY (SurahId) REFERENCES Surah(Id)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE VIRTUAL TABLE IF NOT EXISTS AyahFts USING fts5(
                    Verse, 
                    Translation, 
                    Content='Ayah',
                    Content_Rowid='Id'
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_Ayah_SurahId_AyahNumber ON Ayah(SurahId, AyahNumber);
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS AyatNote (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AyahId1 INTEGER NOT NULL,
                    AyahId2 INTEGER NOT NULL,
                    Note TEXT NOT NULL,
                    FOREIGN KEY (AyahId1) REFERENCES Ayah(Id),
                    FOREIGN KEY (AyahId2) REFERENCES Ayah(Id),
                    UNIQUE (AyahId1, AyahId2)
                );
            ";
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS SurahNote (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SurahId INTEGER NOT NULL,
                    Note TEXT NOT NULL,
                    FOREIGN KEY (SurahId) REFERENCES Surah(Id),
                    UNIQUE (SurahId)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Grouping (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Note TEXT,
                    UNIQUE (Name)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS GroupingLink (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    GroupId INTEGER NOT NULL,
                    AyahId1 INTEGER NOT NULL,
                    AyahId2 INTEGER NOT NULL,
                    FOREIGN KEY (GroupId) REFERENCES Grouping(Id),
                    FOREIGN KEY (AyahId1) REFERENCES Ayah(Id),
                    FOREIGN KEY (AyahId2) REFERENCES Ayah(Id),
                    UNIQUE (AyahId1, AyahId2, GroupId)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS DirectLink (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Note TEXT,
                    AyahId1 INTEGER NOT NULL,
                    AyahId2 INTEGER NOT NULL,
                    AyahId3 INTEGER NOT NULL,
                    AyahId4 INTEGER NOT NULL,
                    FOREIGN KEY (AyahId1) REFERENCES Ayah(Id),
                    FOREIGN KEY (AyahId2) REFERENCES Ayah(Id),
                    FOREIGN KEY (AyahId3) REFERENCES Ayah(Id),
                    FOREIGN KEY (AyahId4) REFERENCES Ayah(Id),
                    UNIQUE (AyahId1, AyahId2, AyahId3, AyahId4)
                );
            ";
            command.ExecuteNonQuery();
        }
    }
}
