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
                CREATE TABLE IF NOT EXISTS SurahNote (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SurahId INTEGER NOT NULL,
                    Text TEXT NOT NULL,
                    FOREIGN KEY (SurahId) REFERENCES Surah(Id)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_SurahNote_SurahId ON SurahNote(SurahId);
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS [Group] (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Text TEXT
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_Group_Name ON [Group](Name);
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Link (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    GroupId INTEGER NOT NULL,
                    AyahId1 INTEGER NOT NULL,
                    AyahId2 INTEGER NOT NULL,
                    FOREIGN KEY (GroupId) REFERENCES [Group](Id),
                    FOREIGN KEY (AyahId1) REFERENCES Ayah(Id),
                    FOREIGN KEY (AyahId2) REFERENCES Ayah(Id),
                    UNIQUE (AyahId1, AyahId2, GroupId)
                );
            ";
            command.ExecuteNonQuery();
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_Link_GroupId1 ON Link(GroupId);
                CREATE INDEX IF NOT EXISTS idx_Link_AyahId1 ON Link(AyahId1);
                CREATE INDEX IF NOT EXISTS idx_Link_AyahId2 ON Link(AyahId2);
            ";
            command.ExecuteNonQuery();
        }
    }
}
