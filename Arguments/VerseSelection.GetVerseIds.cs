using System;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class VerseSelection
    {
        public (int verseId1, int verseId2) GetVerseIds()
        {
            var (verseId1, verseId2) = GetParsedVerseIds();
            if (verseId1 > verseId2) throw new Exception($"Verse ID '{verseId1}' should not be greater than '{verseId2}'");
            return (verseId1, verseId2);
        }

        private (int verseId1, int verseId2) GetParsedVerseIds()
        {
            if (rangeType == RangeType.ChapterFromStart)
            {
                var chapter = Chapter.SelectByNumber(chapterNumber2);
                return (1, chapter.End);
            }
            if (rangeType == RangeType.VerseFromStart)
            {
                var chapterNumberentifier = tokens[0];
                var verseNumber = int.Parse(tokens[1]);
                var verseId = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier, verseNumber);
                return (1, verseId);
            }
            if (rangeType == RangeType.ChapterToEnd)
            {
                var chapter = Chapter.SelectByNumber(chapterNumber1);
                return (chapter.Start, 6236);
            }
            if (rangeType == RangeType.VerseToEnd)
            {
                var chapterNumberentifier = tokens[0];
                var verseNumber = int.Parse(tokens[1]);
                var verseId = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier, verseNumber);
                return (verseId, 6236);
            }
            if (rangeType == RangeType.LeftRange)
            {
                var chapterNumberentifier1 = tokens[0];
                var chapterNumberentifier2 = tokens[1];
                var verseNumber = int.Parse(tokens[2]);
                var chapter = ChapterIdentifierHelpers.GetChapterByIdentifier(chapterNumberentifier1);
                var verseId1 = chapter.Start;
                var verseId2 = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier2, verseNumber);
                return (verseId1, verseId2);
            }
            if (rangeType == RangeType.ChapterToVerse)
            {
                var chapterNumberentifier = tokens[0];
                var verseNumber = int.Parse(tokens[1]);
                var verseId1 = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier, 1);
                var verseId2 = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier, verseNumber);
                return (verseId1, verseId2);
            }
            if (rangeType == RangeType.RightRange)
            {
                var chapterNumberentifier = tokens[0];
                var verseNumber1 = int.Parse(tokens[1]);
                var verseNumber2 = int.Parse(tokens[2]);
                var verseId1 = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier, verseNumber1);
                var verseId2 = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier, verseNumber2);
                return (verseId1, verseId2);
            }
            if (rangeType == RangeType.ChapterToChapter)
            {
                var chapter1 = Chapter.SelectByNumber(chapterNumber1);
                var chapter2 = Chapter.SelectByNumber(chapterNumber2);
                var verseId1 = chapter1.Start;
                var verseId2 = chapter2.End;
                return (verseId1, verseId2);
            }
            if (rangeType == RangeType.VerseToVerse)
            {
                var chapterNumberentifier1 = tokens[0];
                var verseNumber1 = int.Parse(tokens[1]);
                var chapterNumberentifier2 = tokens[2];
                var verseNumber2 = int.Parse(tokens[3]);
                var verseId1 = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier1, verseNumber1);
                var verseId2 = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier2, verseNumber2);
                return (verseId1, verseId2);
            }
            if (mainType == MainType.All) return (1, 6236);
            if (mainType == MainType.Chapter)
            {
                var chapter = Chapter.SelectByNumber(chapterNumber1);
                return (chapter.Start, chapter.End);
            }
            if (mainType == MainType.Verse)
            {
                var chapterNumberentifier = tokens[0];
                var verseNumber = int.Parse(tokens[1]);
                var verseId = ChapterIdentifierHelpers.GetVerseIdByNumbers(chapterNumberentifier, verseNumber);
                return (verseId, verseId);
            }
            throw new Exception("Parse case not found.");
        }
    }
}