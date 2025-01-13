using System.Collections.Generic;
using System.Linq;
using QuranCli.Data.Models;

namespace QuranCli.Arguments
{
    internal partial class IndexedAyatSelection
    {
        public override IEnumerable<Ayah> GetAyat()
        {
            var ayat = base.GetAyat();
            if (!IsIndexed)
            {
                foreach (var ayah in ayat) yield return ayah;
            }
            else if (IsFromStart)
            {
                foreach (var ayah in Take(ayat)) yield return ayah;
            }
            else if (IsToEnd)
            {
                foreach (var ayah in Skip(ayat)) yield return ayah;
            }
            else
            {
                foreach (var ayah in Take(Skip(ayat))) yield return ayah;
            }
        }

        private IEnumerable<Ayah> Skip(IEnumerable<Ayah> ayat)
        {
            // how much you should skip
            var skip = From;
            foreach (var ayah in ayat)
            {
                var words = ayah.Verse.Split(' ');
                if (skip == 0) yield return ayah;
                else if (words.Length <= skip)
                {
                    skip -= words.Length;
                    continue; // skip the entire ayah
                }
                else
                {
                    // yield a truncated ayah
                    var verse = string.Join(' ', words.Skip(skip));
                    ayah.Verse = verse;
                    skip = 0;
                    yield return ayah;
                }
            }
        }
        private IEnumerable<Ayah> Take(IEnumerable<Ayah> ayat)
        {
            // how much you should take
            var take = To - From + 1;
            foreach (var ayah in ayat)
            {
                var words = ayah.Verse.Split(' ');
                if (take == 0) yield break;
                else if (words.Length <= take)
                {
                    take -= words.Length;
                    yield return ayah;
                }
                else
                {
                    // yield a truncated ayah and stop
                    var verse = string.Join(' ', words.Take(take));
                    ayah.Verse = verse;
                    take = 0;
                    yield return ayah;
                    yield break;
                }
            }
        }
    }
}