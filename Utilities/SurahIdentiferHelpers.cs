using System;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    internal static class SurahIdentifierHelpers
    {
        public static int GetSurahIdByIdentifier(Repository repository, string surahIdentifier)
        {
            if (surahIdentifier.IsSurahName()) return repository.GetSurahByName(surahIdentifier).Id;
            return int.Parse(surahIdentifier);
        }

        public static Surah GetSurahByIdentifier(Repository repository, string surahIdentifier)
        {
            if (surahIdentifier.IsSurahName()) return repository.GetSurahByName(surahIdentifier);
            return repository.GetSurahById(int.Parse(surahIdentifier));
        }

        public static int GetAyahIdByOffset(Repository repository, string surahIdentifier, int ayahNumber)
        {
            var surah = GetSurahByIdentifier(repository, surahIdentifier);
            var ayahId = surah.StartAyahId + ayahNumber - 1;
            if (ayahId < surah.StartAyahId || ayahId > surah.EndAyahId) throw new Exception($"No Ayah found for '{surah.Id}:{ayahNumber}'");
            return ayahId;
        }

        public static Ayah GetAyahByOffset(Repository repository, string surahIdentifier, int ayahNumber)
        {
            var surahId = GetSurahIdByIdentifier(repository, surahIdentifier);
            return repository.GetAyahByOffset(surahId, ayahNumber);
        }
    }
}