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
            const string sql = "SELECT * FROM Surah WHERE Id = @id";
            var surah = connection.QuerySingleOrDefault<Surah>(sql, new { id }) ?? throw new Exception($"No Surah found for ID '{id}'");
            return surah;
        }

        public Surah GetSurahByName(string name)
        {
            const string sql = "SELECT * FROM Surah";
            var surahs = connection.Query<Surah>(sql);
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
            const string sql = "SELECT * FROM Ayah WHERE SurahId = @id";
            var ayat = connection.Query<Ayah>(sql, new { id });
            if (ayat.FirstOrDefault() == null) throw new Exception($"No Surah found for ID '{id}'");
            return ayat;
        }

        public IEnumerable<Ayah> GetAyat()
        {
            const string sql = "SELECT * FROM Ayah;";
            return connection.Query<Ayah>(sql);
        }

        public Ayah GetAyahById(int id)
        {
            const string sql = "SELECT * FROM Ayah WHERE Id = @id;";
            return connection.QueryFirstOrDefault<Ayah>(sql, new { id });
        }

        public IEnumerable<Surah> GetSurahs()
        {
            const string sql = "SELECT * FROM Surah";
            return connection.Query<Surah>(sql);
        }

        public Ayah GetAyahByOffset(int surahId, int ayahNumber)
        {
            const string sql = "SELECT * FROM Ayah WHERE SurahId = @surahId AND AyahNumber = @ayahNumber";
            var ayah = connection.QuerySingleOrDefault<Ayah>(sql, new { surahId, ayahNumber }) ?? throw new Exception($"No Ayah found for '{surahId}:{ayahNumber}'");
            return ayah;
        }

        public IEnumerable<Ayah> GetAyatBetweenIds(int ayahId1, int ayahId2)
        {
            if (ayahId2 < ayahId1) throw new Exception("Ending position cannot be less than starting position.");
            const string sql = @"
                SELECT * FROM Ayah
                WHERE Id BETWEEN @ayahId1 AND @ayahId2;
            ";
            var ayat = connection.Query<Ayah>(sql, new { ayahId1, ayahId2 });
            if (ayat.FirstOrDefault() == null) throw new Exception($"No Ayat found between '{ayahId1}..{ayahId2}'");
            return ayat;
        }

        public IEnumerable<Surah> GetSurahsBetweenId(int surahId1, int surahId2)
        {
            if (surahId2 < surahId1) throw new Exception("Ending position cannot be less than starting position.");
            const string sql = @"
                SELECT * FROM Surah
                WHERE Id BETWEEN @surahId1 AND @surahId2;
            ";
            connection.CreateCommand();
            var surahs = connection.Query<Surah>(sql, new { surahId1, surahId2 });
            if (surahs.FirstOrDefault() == null) throw new Exception($"No Surah found between '{surahId1}..{surahId2}'");
            return surahs;
        }

        public string GetDisplayName(int ayahId1, int ayahId2)
        {
            if (ayahId1 == ayahId2) return GetDisplayName(ayahId1);
            var ayah1 = GetAyahById(ayahId1);
            var surah1 = GetSurahById(ayah1.SurahId);
            var ayah2 = GetAyahById(ayahId2);
            var surah2 = GetSurahById(ayah2.SurahId);
            if (surah1.Id == surah2.Id)
            {
                if (ayah1.Id == surah1.StartAyahId && ayah2.Id == surah2.EndAyahId) return surah1.TransliterationName;
                return $"{surah1.TransliterationName} {ayah1.AyahNumber} to {ayah2.AyahNumber}";
            }
            return $"{surah1.TransliterationName} {ayah1.AyahNumber} to {surah2.TransliterationName} {ayah2.AyahNumber}";
        }

        public string GetDisplayName(int ayahId)
        {
            var ayah = GetAyahById(ayahId);
            var surah = GetSurahById(ayah.SurahId);
            return $"{surah.TransliterationName} {ayah.AyahNumber}";
        }

        public string GetSurahDisplayName(int surahId1, int surahId2)
        {
            if (surahId1 == surahId2) return GetSurahById(surahId2).Name;
            else if (surahId1 == 1 && surahId2 == 114) return "القرآن الكريم";
            else return $"{GetSurahById(surahId1).Name} to {GetSurahById(surahId2).Name}";
        }

        public string GetSurahTransliterationDisplayName(int surahId1, int surahId2)
        {
            if (surahId1 == surahId2) return GetSurahById(surahId2).TransliterationName;
            else if (surahId1 == 1 && surahId2 == 114) return "The Noble Quran";
            else return $"{GetSurahById(surahId1).TransliterationName} to {GetSurahById(surahId2).TransliterationName}";
        }
    }
}