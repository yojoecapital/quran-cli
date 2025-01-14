using QuranCli.Data.Models;
using QuranCli.Utilities;
using Dapper;

namespace QuranCli.Data
{
    internal partial class Repository
    {
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