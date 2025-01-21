using System.Collections.Generic;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class VerseSelection
    {
        public virtual IEnumerable<Verse> GetVerses()
        {
            if (mainType == MainType.Chapter)
            {
                var chapterNumberentifier = tokens[0];
                var chapterNumber = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(chapterNumberentifier);
                return Verse.SelectByNumber(chapterNumber);
            }
            if (mainType == MainType.Verse)
            {
                var chapterNumberentifier = tokens[0];
                var verseNumber = int.Parse(tokens[1]);
                return [ChapterIdentifierHelpers.GetVerseByOffset(chapterNumberentifier, verseNumber)];
            }
            var (verseId1, verseId2) = GetVerseIds();
            return Verse.SelectBetweenIds(verseId1, verseId2);
        }
    }
}