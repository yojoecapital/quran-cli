using System;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    internal static class SurahIdentifierHelpers
    {
        public static int GetSurahIdByIdentifier(string surahIdentifier)
        {
            if (surahIdentifier.IsSurahName()) return Repository.Instance.GetSurahByName(surahIdentifier).Id;
            return int.Parse(surahIdentifier);
        }

        public static Surah GetSurahByIdentifier(string surahIdentifier)
        {
            if (surahIdentifier.IsSurahName()) return Repository.Instance.GetSurahByName(surahIdentifier);
            return Repository.Instance.GetSurahById(int.Parse(surahIdentifier));
        }

        public static int GetAyahIdByOffset(string surahIdentifier, int ayahNumber)
        {
            var surah = GetSurahByIdentifier(surahIdentifier);
            var ayahId = surah.StartAyahId + ayahNumber - 1;
            if (ayahId < surah.StartAyahId || ayahId > surah.EndAyahId) throw new Exception($"No Ayah found for '{surah.Id}:{ayahNumber}'");
            return ayahId;
        }

        public static Ayah GetAyahByOffset(string surahIdentifier, int ayahNumber)
        {
            var surahId = GetSurahIdByIdentifier(surahIdentifier);
            return Repository.Instance.GetAyahByOffset(surahId, ayahNumber);
        }
    }
}