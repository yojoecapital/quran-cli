using System.Collections.Generic;
using Dapper;
using QuranCli.Data.Models;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public IEnumerable<SurahNote> GetSurahNotesBetween(int surahId1, int surahId2)
        {
            const string query = "SELECT * FROM SurahNote WHERE SurahId1 <= @surahId2 AND SurahId2 >= @surahId1;";
            return connection.Query<SurahNote>(query, new { surahId1, surahId2 });
        }

        public void CreateOrEdit(SurahNote surahNote)
        {
            const string sql = @"
                INSERT OR REPLACE INTO SurahNote (SurahId1, SurahId2, Note)
                VALUES (@SurahId1, @SurahId2, @Note)
            ";
            connection.Execute(sql, surahNote);
        }

        public void DeleteSurahNote(int id)
        {
            const string sql = @"
                DELETE FROM SurahNote
                WHERE Id = @id;
            ";
            connection.Execute(sql, new { id });
        }
    }
}