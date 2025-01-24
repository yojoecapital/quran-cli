using System.Collections.Generic;
using QuranCli.Data.Models;

namespace QuranCli.Arguments
{
    public partial class ChapterSelection
    {
        public IEnumerable<Chapter> GetChapters() => Chapter.SelectBetweenNumbers(ChapterNumber1, ChapterNumber2);
    }
}