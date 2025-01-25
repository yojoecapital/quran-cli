using System;
using System.Diagnostics.Contracts;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    public static class SelectionHelpers
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

        public static int GetVerseIdByNumbers(string chapterNumberentifier, int verseNumber)
        {
            var chapter = GetChapterByIdentifier(chapterNumberentifier);
            var verseId = chapter.Start + verseNumber - 1;
            if (verseId < chapter.Start || verseId > chapter.End) throw new Exception($"No Verse found for '{chapter.Number}:{verseNumber}'");
            return verseId;
        }

        public static (int verseId1, int verseId2) GetVerseIdsByPageOrJuzIdentifier(string pageOrJuzIdentifier)
        {
            if (pageOrJuzIdentifier[0] == 'j')
            {
                var juzNumber = int.Parse(pageOrJuzIdentifier[1..]);
                if (juzNumber < 1 || juzNumber > 30) throw new Exception($"Invalid Juz number 'j{juzNumber}'");
                int start, end;
                if (juzNumber == 1)
                {
                    start = 1;
                    end = 21;
                }
                else if (juzNumber == 30)
                {
                    start = 582;
                    end = 604;
                }
                else
                {
                    start = 2 + (juzNumber - 1) * 20;
                    end = start + 20;
                }
                var startPage = Page.SelectByNumber(start);
                var endPage = Page.SelectByNumber(end);
                Logger.Info(endPage.Number);
                return (startPage.Start, endPage.End);
            }
            else
            {
                var pageNumber = int.Parse(pageOrJuzIdentifier[1..]);
                var page = Page.SelectByNumber(pageNumber);
                return (page.Start, page.End);
            }
        }
    }
}