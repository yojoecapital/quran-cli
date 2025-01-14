using QuranCli.Data.Models;
using QuranCli.Utilities;
using Dapper;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public void PopulateAyahFts()
        {
            const string sql = @"
                INSERT INTO AyahFts(rowid, Verse, Translation)
                SELECT Id, Verse, Translation FROM Ayah
                WHERE Id NOT IN (SELECT rowid FROM AyahFts);
            ";
            connection.Execute(sql);
        }

        public void CreateTables() => connection.CreateTables();

        public void Create(Ayah ayah)
        {
            const string sql = @"
                INSERT INTO Ayah (Id, SurahId, AyahNumber, Verse, Translation) 
                VALUES (@Id, @SurahId, @AyahNumber, @Verse, @Translation);
            ";
            connection.Execute(sql, ayah);
        }

        public void Create(Surah surah)
        {
            const string sql = @"
                INSERT INTO Surah (Id, AyahCount, StartAyahId, Name, EnglishName, TransliterationName) 
                VALUES (@Id, @AyahCount, @StartAyahId, @Name, @EnglishName, @TransliterationName);
            ";
            connection.Execute(sql, surah);
        }
    }
}