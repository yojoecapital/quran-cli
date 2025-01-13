using QuranCli.Data.Models;
using QuranCli.Utilities;
using System;
using Dapper;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public void PopulateAyahFts()
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO AyahFts(rowid, Verse, Translation)
                SELECT Id, Verse, Translation FROM Ayah
                WHERE Id NOT IN (SELECT rowid FROM AyahFts);
            ";
            command.ExecuteNonQuery();
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

        public void Create(SurahNote surahNote)
        {
            const string sql = @"
                INSERT INTO SurahNote (SurahId, Text) 
                VALUES (@SurahId, @Text);
            ";
            connection.Execute(sql, surahNote);
        }

        public int Create(Group node)
        {
            const string sql = @"
                INSERT INTO [Group] (Name, Text) 
                VALUES (@Name, @Text);
                SELECT LAST_INSERT_ROWID();
            ";
            return connection.QuerySingle<int>(sql, node);
        }


        public void Create(Link link)
        {
            const string sql = @"
                INSERT INTO Link (GroupId, AyahId1, AyahId2) 
                VALUES (@GroupId, @AyahId1, @AyahId2);
            ";
            connection.Execute(sql, link);
        }

    }
}