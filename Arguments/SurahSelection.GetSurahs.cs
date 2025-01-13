using System;
using System.Collections.Generic;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    internal partial class SurahSelection
    {
        public IEnumerable<Surah> GetSurahs(Repository repository)
        {
            Log();
            if (type == Type.All)
            {
                return repository.GetSurahs();
            }
            if (type == Type.Surah)
            {
                var surahIdentifier = tokens[0];
                var surah = SurahIdentifierHelpers.GetSurahByIdentifier(repository, surahIdentifier);
                return [surah];
            }
            if (type == Type.SurahFromStart)
            {
                var surahIdentifier = tokens[0];
                var surahId = SurahIdentifierHelpers.GetSurahIdByIdentifier(repository, surahIdentifier);
                return repository.GetSurahsBetweenId(1, surahId);
            }
            if (type == Type.SurahToEnd)
            {
                var surahIdentifier = tokens[0];
                var surahId = SurahIdentifierHelpers.GetSurahIdByIdentifier(repository, surahIdentifier);
                return repository.GetSurahsBetweenId(surahId, 114);
            }
            if (type == Type.SurahToSurah)
            {
                var surahIdentifier1 = tokens[0];
                var surahIdentifier2 = tokens[1];
                var surahId1 = SurahIdentifierHelpers.GetSurahIdByIdentifier(repository, surahIdentifier1);
                var surahId2 = SurahIdentifierHelpers.GetSurahIdByIdentifier(repository, surahIdentifier2);
                return repository.GetSurahsBetweenId(surahId1, surahId2);
            }
            throw new Exception("Parse case not found.");
        }
    }
}