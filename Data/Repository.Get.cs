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
            var ranking = surahs
                .Select(s => new RankedSurah
                {
                    Surah = s,
                    Distance = StringExtensions.ComputeLevenshteinDistance(name, s.TransliterationName)
                })
                .OrderBy(rs => rs.Distance)
                .FirstOrDefault();
            return ranking.Surah;
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
                WHERE id BETWEEN @ayahId1 AND @ayahId2
                ORDER BY id;
            ";
            var ayat = connection.Query<Ayah>(query, new { ayahId1, ayahId2 });
            if (ayat.FirstOrDefault() == null) throw new Exception($"No Ayat found between '{ayahId1}..{ayahId2}'");
            return ayat;
        }

    }
}