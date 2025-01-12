using QuranCli.Data.Models;
using QuranCli.Utilities;
using System;
using Dapper;

namespace QuranCli.Data
{
    internal partial class Repository : IDisposable
    {
        public void PopulateAyahFts()
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO AyahFts(rowid, verse, translation)
                SELECT id, verse, translation FROM Ayah
                WHERE id NOT IN (SELECT rowid FROM AyahFts);
            ";
            command.ExecuteNonQuery();
        }

        public void CreateTables() => connection.CreateTables();

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