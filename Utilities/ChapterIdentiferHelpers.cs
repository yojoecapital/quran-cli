using System;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    public static class ChapterIdentifierHelpers
    {
        public static int GetChapterNumberByIdentifier(string chapterNumberentifier)
        {
            if (chapterNumberentifier.IsChapterTransliteration()) return Chapter.SelectByTransliteration(chapterNumberentifier).Number;
            return int.Parse(chapterNumberentifier);
        }

        public static Chapter GetChapterByIdentifier(string chapterNumberentifier)
        {
            if (chapterNumberentifier.IsChapterTransliteration()) return Chapter.SelectByTransliteration(chapterNumberentifier);
            return Chapter.SelectByNumber(int.Parse(chapterNumberentifier));
        }

        public static int GetVerseIdByOffset(string chapterNumberentifier, int verseNumber)
        {
            var chapter = GetChapterByIdentifier(chapterNumberentifier);
            var verseId = chapter.Start + verseNumber - 1;
            if (verseId < chapter.Start || verseId > chapter.End) throw new Exception($"No Verse found for '{chapter.Number}:{verseNumber}'");
            return verseId;
        }

        public static Verse GetVerseByOffset(string chapterNumberentifier, int verseNumber)
        {
            var chapterNumber = GetChapterNumberByIdentifier(chapterNumberentifier);
            return Verse.SelectByNumber(chapterNumber, verseNumber);
        }
    }
}