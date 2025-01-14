using System.Collections.Generic;
using System.Linq;
using Dapper;
using QuranCli.Data.Models;
using SQLitePCL;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public void CreateOrEdit(DirectLink directLink)
        {
            const string sql = @"
                INSERT OR REPLACE INTO DirectLink (AyahId1, AyahId2, AyahId3, AyahId4, Note)
                VALUES (@AyahId1, @AyahId2, @AyahId3, @AyahId4, @Note)
            ";
            connection.Execute(sql, directLink);
        }

        public void DeleteDirectLink(int id)
        {
            const string sql = @"
                DELETE FROM DirectLink 
                WHERE Id == @id;
            ";
            connection.Execute(sql, new { id });
        }

        public IEnumerable<DirectLink> GetDirectLinksBetween(int ayahId1, int ayahId2)
        {
            const string query = @"
                SELECT * FROM DirectLink 
                WHERE AyahId1 <= @ayahId2 AND AyahId2 >= @ayahId1
                OR AyahId3 <= @ayahId2 AND AyahId4 >= @ayahId1;
            ";
            return connection.Query<DirectLink>(query, new { ayahId1, ayahId2 });
        }

        public IEnumerable<DirectLink> GetDirectLinksBetween(int ayahId1, int ayahId2, int ayahId3, int ayahId4)
        {
            const string query = @"
                SELECT * FROM DirectLink 
                WHERE AyahId1 <= @ayahId2 AND AyahId2 >= @ayahId1
                AND AyahId3 <= @ayahId4 AND AyahId4 >= @ayahId3;
            ";
            return connection.Query<DirectLink>(query, new { ayahId1, ayahId2, ayahId3, ayahId4 });
        }
    }
}