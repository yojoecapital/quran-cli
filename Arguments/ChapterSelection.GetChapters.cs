using System;
using System.Collections.Generic;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class ChapterSelection
    {
        public IEnumerable<Chapter> GetChapters()
        {
            if (type == Type.All)
            {
                return Chapter.SelectAll();
            }
            if (type == Type.Chapter)
            {
                var chapterNumberentifier = tokens[0];
                var chapter = ChapterIdentifierHelpers.GetChapterByIdentifier(chapterNumberentifier);
                return [chapter];
            }
            if (type == Type.ChapterFromStart)
            {
                var chapterNumberentifier = tokens[0];
                var chapterNumber = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(chapterNumberentifier);
                return Chapter.SelectBetweenNumbers(1, chapterNumber);
            }
            if (type == Type.ChapterToEnd)
            {
                var chapterNumberentifier = tokens[0];
                var chapterNumber = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(chapterNumberentifier);
                return Chapter.SelectBetweenNumbers(chapterNumber, 114);
            }
            if (type == Type.ChapterToChapter)
            {
                var chapterNumberentifier1 = tokens[0];
                var chapterNumberentifier2 = tokens[1];
                var chapterNumber1 = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(chapterNumberentifier1);
                var chapterNumber2 = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(chapterNumberentifier2);
                if (chapterNumber1 > chapterNumber2) throw new Exception($"Chapter number {chapterNumber1} should not be greater than {chapterNumber2}");
                return Chapter.SelectBetweenNumbers(chapterNumber1, chapterNumber2);
            }
            throw new Exception("Parse case not found");
        }
    }
}