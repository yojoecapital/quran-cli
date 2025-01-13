using System.Collections.Generic;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    internal partial class AyatSelection
    {
        public virtual IEnumerable<Ayah> GetAyat()
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
            var (ayahId1, ayahId2) = GetAyahIds();
            return Repository.Instance.GetAyatBetweenIds(ayahId1, ayahId2);
        }
    }
}