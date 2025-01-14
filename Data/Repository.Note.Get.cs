using System.Collections.Generic;
using Dapper;
using QuranCli.Data.Models;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public IEnumerable<AyatNote> GetAyatNotesBetween(int ayahId1, int ayahId2)
        {
            const string query = "SELECT * FROM AyatNote WHERE AyahId1 <= @ayahId2 AND AyahId2 >= @ayahId1";
            return connection.Query<AyatNote>(query, new { ayahId1, ayahId2 });
        }

        public AyatNote GetAyatNote(int ayahId1, int ayahId2)
        {
            const string query = "SELECT * FROM AyatNote WHERE AyahId1 == @ayahId1 AND AyahId2 == @ayahId2";
            return connection.QueryFirstOrDefault<AyatNote>(query, new { ayahId1, ayahId2 });
        }
    }
}