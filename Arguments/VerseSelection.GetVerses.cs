using System.Collections.Generic;
using QuranCli.Data.Models;

namespace QuranCli.Arguments
{
    public partial class VerseSelection
    {
        public virtual IEnumerable<Verse> GetVerses() => Verse.SelectBetweenIds(VerseId1, VerseId2);
    }
}