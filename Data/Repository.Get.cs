using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public Surah GetSurahById(int id)
        {
            const string query = "SELECT * FROM Surah WHERE id = @id";
            var surah = connection.QuerySingleOrDefault<Surah>(query, new { id }) ?? throw new Exception($"No Surah found for ID '{id}'");
            return surah;
        }

        public Surah GetSurahByName(string name)
        {
            const string query = "SELECT * FROM Surah";
            var surahs = connection.Query<Surah>(query);
            var rankings = surahs
                .Select(s => new RankedSurah
                {
                    Surah = s,
                    Distance = StringExtensions.ComputeLevenshteinDistance(name, s.TransliterationName)
                })
                .OrderBy(rs => rs.Distance);
            return rankings.First().Surah;
        }

        public IEnumerable<Ayah> GetAyatInSurahById(int id)
        {
            const string query = "SELECT * FROM Ayah WHERE surahId = @id";
            var ayat = connection.Query<Ayah>(query, new { id });
            if (ayat.FirstOrDefault() == null) throw new Exception($"No Surah found for ID '{id}'");
            return ayat;
        }

        public IEnumerable<Ayah> GetAyat()
        {
            const string query = "SELECT * FROM Ayah";
            return connection.Query<Ayah>(query);
        }

        public IEnumerable<Surah> GetSurahs()
        {
            const string query = "SELECT * FROM Surah";
            return connection.Query<Surah>(query);
        }

        public Ayah GetAyahByOffset(int surahId, int ayahNumber)
        {
            const string query = "SELECT * FROM Ayah WHERE surahId = @surahId AND ayahNumber = @ayahNumber";
            var ayah = connection.QuerySingleOrDefault<Ayah>(query, new { surahId, ayahNumber }) ?? throw new Exception($"No Ayah found for '{surahId}:{ayahNumber}'");
            return ayah;
        }

        public IEnumerable<Ayah> GetAyatBetweenIds(int ayahId1, int ayahId2)
        {
            if (ayahId2 < ayahId1) throw new Exception("Ending position cannot be less than starting position.");
            const string query = @"
                SELECT * FROM Ayah
                WHERE id BETWEEN @ayahId1 AND @ayahId2;
            ";
            var ayat = connection.Query<Ayah>(query, new { ayahId1, ayahId2 });
            if (ayat.FirstOrDefault() == null) throw new Exception($"No Ayat found between '{ayahId1}..{ayahId2}'");
            return ayat;
        }

        public IEnumerable<Surah> GetSurahsBetweenId(int surahId1, int surahId2)
        {
            if (surahId2 < surahId1) throw new Exception("Ending position cannot be less than starting position.");
            const string query = @"
                SELECT * FROM Surah
                WHERE id BETWEEN @surahId1 AND @surahId2;
            ";
            var surahs = connection.Query<Surah>(query, new { surahId1, surahId2 });
            if (surahs.FirstOrDefault() == null) throw new Exception($"No Surah found between '{surahId1}..{surahId2}'");
            return surahs;
        }

        public bool TryGetGroupByName(string name, out Group group)
        {
            const string query = @"
                SELECT * FROM Group
                WHERE name = @name;
            ";
            group = connection.QuerySingleOrDefault<Group>(query, new { name });
            if (group == null) return false;
            return true;
        }

        public IEnumerable<Link> GetLinksBetweenAyahIds(int ayahId1, int ayahId2)
        {
            if (ayahId2 < ayahId1) throw new Exception("Ending position cannot be less than starting position.");
            const string query = @"
                SELECT * 
                FROM Link
                WHERE NOT (ayahId2 < @ayahId1 OR ayahId1 > @ayahId1)
            ";
            return connection.Query<Link>(query, new { ayahId1, ayahId2 });
        }

        public Link GetLinkByAyahIds(int ayahId1, int ayahId2)
        {
            if (ayahId2 < ayahId1) throw new Exception("Ending position cannot be less than starting position.");
            const string query = @"
                SELECT * 
                FROM Link
                WHERE ayahId2 = @ayahId1 AND ayahId1 = @ayahId1
            ";
            return connection.QuerySingleOrDefault<Link>(query, new { ayahId1, ayahId2 });
        }
    }
}