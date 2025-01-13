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
            const string query = "SELECT * FROM Surah WHERE Id = @id";
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
            const string query = "SELECT * FROM Ayah WHERE SurahId = @id";
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
            const string query = "SELECT * FROM Ayah WHERE SurahId = @surahId AND AyahNumber = @ayahNumber";
            var ayah = connection.QuerySingleOrDefault<Ayah>(query, new { surahId, ayahNumber }) ?? throw new Exception($"No Ayah found for '{surahId}:{ayahNumber}'");
            return ayah;
        }

        public IEnumerable<Ayah> GetAyatBetweenIds(int ayahId1, int ayahId2)
        {
            if (ayahId2 < ayahId1) throw new Exception("Ending position cannot be less than starting position.");
            const string query = @"
                SELECT * FROM Ayah
                WHERE Id BETWEEN @ayahId1 AND @ayahId2;
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
                WHERE Id BETWEEN @surahId1 AND @surahId2;
            ";
            var surahs = connection.Query<Surah>(query, new { surahId1, surahId2 });
            if (surahs.FirstOrDefault() == null) throw new Exception($"No Surah found between '{surahId1}..{surahId2}'");
            return surahs;
        }

        public bool TryGetGroupByName(string name, out Group group)
        {
            const string query = @"
                SELECT * FROM [Group]
                WHERE Name = @name;
            ";
            group = connection.QuerySingleOrDefault<Group>(query, new { name });
            if (group == null) return false;
            return true;
        }

        private Dictionary<int, Link>.ValueCollection GetLinksByAyahIds(string where, int ayahId1, int ayahId2)
        {
            string query = @$"
                SELECT 
                    l1.Id as LinkId,
                    l1.GroupId,
                    l1.AyahId1,
                    l1.AyahId2,
                    g.Id as GroupId,
                    g.Name,
                    g.Text,
                    l2.Id as NestedLinkId,
                    l2.GroupId as NestedLinkGroupId,
                    l2.AyahId1 as NestedLinkAyahId1,
                    l2.AyahId2 as NestedLinkAyahId2
                FROM Link l1
                INNER JOIN [Group] g ON l1.GroupId = g.Id
                LEFT JOIN Link l2 ON g.Id = l2.GroupId
                {where};
            ";
            if (ayahId2 < ayahId1) throw new Exception("Ending position cannot be less than starting position.");
            var linkDictionary = new Dictionary<int, Link>();
            var links = connection.Query<Link, Group, NestedLink, Link>(
                query,
                (link, group, nestedLink) =>
                {
                    if (!linkDictionary.TryGetValue(link.Id, out var existingLink))
                    {
                        existingLink = link;
                        existingLink.Group = group;
                        group.Links = [];
                        linkDictionary.Add(link.Id, existingLink);
                    }
                    if (nestedLink != null && group.Id == nestedLink.NestedLinkGroupId)
                    {
                        group.Links.Add(nestedLink);
                    }
                    return existingLink;
                },
                new { ayahId1, ayahId2 },
                splitOn: "GroupId, NestedLinkId"
            );
            return linkDictionary.Values;
        }

        public IEnumerable<Link> GetLinksBetweenAyahIds(int ayahId1, int ayahId2)
        {
            const string where = "WHERE l1.AyahId1 <= @ayahId2 AND l1.AyahId2 >= @ayahId1";
            return GetLinksByAyahIds(where, ayahId1, ayahId2);
        }

        public IEnumerable<Link> GetLinksByAyahIds(int ayahId1, int ayahId2)
        {
            const string where = "WHERE l1.AyahId1 == @ayahId1 AND l1.AyahId2 == @ayahId2";
            return GetLinksByAyahIds(where, ayahId1, ayahId2);
        }

    }
}