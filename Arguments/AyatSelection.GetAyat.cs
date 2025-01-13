using System;
using System.Collections.Generic;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    internal partial class AyatSelection
    {
        public IEnumerable<Ayah> GetAyat()
        {
            if (mainType == MainType.All) return Repository.Instance.GetAyat();
            if (mainType == MainType.Surah)
            {
                var surahIdentifier = tokens[0];
                var surahId = SurahIdentifierHelpers.GetSurahIdByIdentifier(surahIdentifier);
                return Repository.Instance.GetAyatInSurahById(surahId);
            }
            if (mainType == MainType.Ayah)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber = int.Parse(tokens[1]);
                return [SurahIdentifierHelpers.GetAyahByOffset(surahIdentifier, ayahNumber)];
            }
            if (rangeType == RangeType.SurahFromStart)
            {
                var surahIdentifier = tokens[0];
                var surah = SurahIdentifierHelpers.GetSurahByIdentifier(surahIdentifier);
                return Repository.Instance.GetAyatBetweenIds(1, surah.EndAyahId);
            }
            if (rangeType == RangeType.AyahFromStart)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber = int.Parse(tokens[1]);
                var ayahId = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber);
                return Repository.Instance.GetAyatBetweenIds(1, ayahId);
            }
            if (rangeType == RangeType.SurahToEnd)
            {
                var surahIdentifier = tokens[0];
                var surah = SurahIdentifierHelpers.GetSurahByIdentifier(surahIdentifier);
                return Repository.Instance.GetAyatBetweenIds(surah.StartAyahId, 6236);
            }
            if (rangeType == RangeType.AyahToEnd)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber = int.Parse(tokens[1]);
                var ayahId = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber);
                return Repository.Instance.GetAyatBetweenIds(ayahId, 6236);
            }
            if (rangeType == RangeType.LeftRange)
            {
                var surahIdentifier1 = tokens[0];
                var surahIdentifier2 = tokens[1];
                var ayahNumber = int.Parse(tokens[2]);
                var surah = SurahIdentifierHelpers.GetSurahByIdentifier(surahIdentifier1);
                var ayahId1 = surah.StartAyahId;
                var ayahId2 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier2, ayahNumber);
                return Repository.Instance.GetAyatBetweenIds(ayahId1, ayahId2);
            }
            if (rangeType == RangeType.SurahToAyah)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber = int.Parse(tokens[1]);
                var ayahId1 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, 1);
                var ayahId2 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber);
                return Repository.Instance.GetAyatBetweenIds(ayahId1, ayahId2);
            }
            if (rangeType == RangeType.RightRange)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber1 = int.Parse(tokens[1]);
                var ayahNumber2 = int.Parse(tokens[2]);
                var ayahId1 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber1);
                var ayahId2 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber2);
                return Repository.Instance.GetAyatBetweenIds(ayahId1, ayahId2);
            }
            if (rangeType == RangeType.SurahToSurah)
            {
                var surahIdentifier1 = tokens[0];
                var surahIdentifier2 = tokens[1];
                var surah1 = SurahIdentifierHelpers.GetSurahByIdentifier(surahIdentifier1);
                var surah2 = SurahIdentifierHelpers.GetSurahByIdentifier(surahIdentifier2);
                var ayahId1 = surah1.StartAyahId;
                var ayahId2 = surah2.EndAyahId;
                return Repository.Instance.GetAyatBetweenIds(ayahId1, ayahId2);
            }
            if (rangeType == RangeType.AyahToAyah)
            {
                var surahIdentifier1 = tokens[0];
                var ayahNumber1 = int.Parse(tokens[1]);
                var surahIdentifier2 = tokens[2];
                var ayahNumber2 = int.Parse(tokens[3]);
                var ayahId1 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier1, ayahNumber1);
                var ayahId2 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier2, ayahNumber2);
                return Repository.Instance.GetAyatBetweenIds(ayahId1, ayahId2);
            }
            throw new Exception("Parse case not found.");
        }
    }
}