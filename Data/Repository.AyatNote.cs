using System.Collections.Generic;
using System.Linq;
using Dapper;
using QuranCli.Data.Models;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public IEnumerable<AyatNote> GetAyatNotesBetween(int ayahId1, int ayahId2)
        {
            const string query = "SELECT * FROM AyatNote WHERE AyahId1 <= @ayahId2 AND AyahId2 >= @ayahId1;";
            return connection.Query<AyatNote>(query, new { ayahId1, ayahId2 });
        }

        public void CreateOrEdit(AyatNote ayatNote)
        {
            const string sql = @"
                INSERT OR REPLACE INTO AyatNote (AyahId1, AyahId2, Note)
                VALUES (@AyahId1, @AyahId2, @Note)
            ";
            connection.Execute(sql, ayatNote);
        }

        public void DeleteAyatNote(int id)
        {
            const string sql = @"
                DELETE FROM AyatNote
                WHERE Id = @id;
            ";
            connection.Execute(sql, new { id });
        }
    }
}