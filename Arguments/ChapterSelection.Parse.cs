using System;
using System.CommandLine.Parsing;
using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class ChapterSelection
    {
        public int ChapterNumber1 { get; private set; }
        public int ChapterNumber2 { get; private set; }

        public static bool TryParse(string value, out ChapterSelection selection)
        {
            selection = new();
            return selection.TryGetTokens(value.Trim().ToLower());
        }

        private bool TryGetTokens(string value)
        {
            var splitArity = Splitter.GetSplit(value, "..", out var split);
            if (splitArity == Splitter.Arity.One)
            {
                if (split.First.Equals("all"))
                {
                    ChapterNumber1 = 1;
                    ChapterNumber2 = 114;
                    return true;
                }
                if (split.First.IsChapterIdentifier())
                {
                    ChapterNumber1 = ChapterNumber2 = SelectionHelpers.GetChapterNumberByIdentifier(split.First);
                    return true;
                }
            }
            else if (splitArity == Splitter.Arity.Two)
            {
                if (split.First.Length == 0 && split.Last.IsChapterIdentifier())
                {
                    ChapterNumber1 = 1;
                    ChapterNumber2 = SelectionHelpers.GetChapterNumberByIdentifier(split.Last);
                    return true;
                }
                if (split.First.IsChapterIdentifier() && split.Last.Length == 0)
                {
                    ChapterNumber1 = SelectionHelpers.GetChapterNumberByIdentifier(split.First);
                    ChapterNumber2 = 114;
                    return true;
                }
                if (split.First.IsChapterIdentifier() && split.Last.IsChapterIdentifier())
                {
                    ChapterNumber1 = SelectionHelpers.GetChapterNumberByIdentifier(split.First);
                    ChapterNumber2 = SelectionHelpers.GetChapterNumberByIdentifier(split.Last);
                    return true;
                }
            }
            return false;
        }
    }
}