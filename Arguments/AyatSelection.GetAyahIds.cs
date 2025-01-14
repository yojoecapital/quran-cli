using System;
using QuranCli.Data;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    internal partial class AyatSelection
    {
        public (int ayahId1, int ayahId2) GetAyahIds()
        {
            if (rangeType == RangeType.SurahFromStart)
            {
                var surah = Repository.Instance.GetSurahById(surahId2);
                return (1, surah.EndAyahId);
            }
            if (rangeType == RangeType.AyahFromStart)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber = int.Parse(tokens[1]);
                var ayahId = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber);
                return (1, ayahId);
            }
            if (rangeType == RangeType.SurahToEnd)
            {
                var surah = Repository.Instance.GetSurahById(surahId1);
                return (surah.StartAyahId, 6236);
            }
            if (rangeType == RangeType.AyahToEnd)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber = int.Parse(tokens[1]);
                var ayahId = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber);
                return (ayahId, 6236);
            }
            if (rangeType == RangeType.LeftRange)
            {
                var surahIdentifier1 = tokens[0];
                var surahIdentifier2 = tokens[1];
                var ayahNumber = int.Parse(tokens[2]);
                var surah = SurahIdentifierHelpers.GetSurahByIdentifier(surahIdentifier1);
                var ayahId1 = surah.StartAyahId;
                var ayahId2 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier2, ayahNumber);
                return (ayahId1, ayahId2);
            }
            if (rangeType == RangeType.SurahToAyah)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber = int.Parse(tokens[1]);
                var ayahId1 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, 1);
                var ayahId2 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber);
                return (ayahId1, ayahId2);
            }
            if (rangeType == RangeType.RightRange)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber1 = int.Parse(tokens[1]);
                var ayahNumber2 = int.Parse(tokens[2]);
                var ayahId1 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber1);
                var ayahId2 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber2);
                return (ayahId1, ayahId2);
            }
            if (rangeType == RangeType.SurahToSurah)
            {
                var surah1 = Repository.Instance.GetSurahById(surahId1);
                var surah2 = Repository.Instance.GetSurahById(surahId2);
                var ayahId1 = surah1.StartAyahId;
                var ayahId2 = surah2.EndAyahId;
                return (ayahId1, ayahId2);
            }
            if (rangeType == RangeType.AyahToAyah)
            {
                var surahIdentifier1 = tokens[0];
                var ayahNumber1 = int.Parse(tokens[1]);
                var surahIdentifier2 = tokens[2];
                var ayahNumber2 = int.Parse(tokens[3]);
                var ayahId1 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier1, ayahNumber1);
                var ayahId2 = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier2, ayahNumber2);
                return (ayahId1, ayahId2);
            }
            if (mainType == MainType.All) return (1, 6236);
            if (mainType == MainType.Surah)
            {
                var surah = Repository.Instance.GetSurahById(surahId1);
                return (surah.StartAyahId, surah.EndAyahId);
            }
            if (mainType == MainType.Ayah)
            {
                var surahIdentifier = tokens[0];
                var ayahNumber = int.Parse(tokens[1]);
                var ayahId = SurahIdentifierHelpers.GetAyahIdByOffset(surahIdentifier, ayahNumber);
                return (ayahId, ayahId);
            }
            throw new Exception("Parse case not found.");
        }
    }
}