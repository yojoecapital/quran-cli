using System.Collections.Generic;
using System.Linq;
using QuranCli.Data.Models;

namespace QuranCli.Arguments
{
    public partial class IndexedVerseSelection
    {
        public override IEnumerable<Verse> GetVerses()
        {
            var verses = base.GetVerses();
            if (!IsIndexed)
            {
                foreach (var verse in verses) yield return verse;
            }
            else if (IsFromStart)
            {
                foreach (var verse in Take(verses)) yield return verse;
            }
            else if (IsToEnd)
            {
                foreach (var verse in Skip(verses)) yield return verse;
            }
            else
            {
                foreach (var verse in Take(Skip(verses))) yield return verse;
            }
        }

        private IEnumerable<Verse> Skip(IEnumerable<Verse> verses)
        {
            // how much you should skip
            var skip = From;
            foreach (var verse in verses)
            {
                var words = verse.Text.Split(' ');
                if (skip == 0) yield return verse;
                else if (words.Length <= skip)
                {
                    skip -= words.Length;
                    continue; // skip the entire verse
                }
                else
                {
                    // yield a truncated verse
                    var text = string.Join(' ', words.Skip(skip));
                    verse.Text = text;
                    skip = 0;
                    yield return verse;
                }
            }
        }
        private IEnumerable<Verse> Take(IEnumerable<Verse> verses)
        {
            // how much you should take
            var take = To - From + 1;
            foreach (var verse in verses)
            {
                var words = verse.Text.Split(' ');
                if (take == 0) yield break;
                else if (words.Length <= take)
                {
                    take -= words.Length;
                    yield return verse;
                }
                else
                {
                    // yield a truncated verse and stop
                    verse.Text = string.Join(' ', words.Take(take));
                    take = 0;
                    yield return verse;
                    yield break;
                }
            }
        }
    }
}