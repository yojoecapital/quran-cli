using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class VerseSelection
    {
        protected VerseSelection
        (
            MainType mainType,
            RangeType rangeType,
            string[] tokens
        )
        {
            this.mainType = mainType;
            this.rangeType = rangeType;
            this.tokens = tokens;
            if (mainType == MainType.All)
            {
                chapterNumber1 = 1;
                chapterNumber2 = 114;
                isChapterSelection = true;
            }
            else if (mainType == MainType.Chapter)
            {
                chapterNumber1 = chapterNumber2 = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(tokens[0]);
                isChapterSelection = true;
            }
            else if (mainType == MainType.Range)
            {
                if (rangeType == RangeType.ChapterFromStart)
                {
                    chapterNumber1 = 1;
                    chapterNumber2 = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(tokens[0]);
                    isChapterSelection = true;
                }
                else if (rangeType == RangeType.ChapterToEnd)
                {
                    chapterNumber1 = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(tokens[0]);
                    chapterNumber2 = 114;
                    isChapterSelection = true;
                }
                else if (rangeType == RangeType.ChapterToChapter)
                {
                    chapterNumber1 = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(tokens[0]);
                    chapterNumber2 = ChapterIdentifierHelpers.GetChapterNumberByIdentifier(tokens[1]);
                    isChapterSelection = true;
                }
            }
        }

        public readonly int chapterNumber1;
        public readonly int chapterNumber2;
        public readonly bool isChapterSelection;

        public virtual string GetLog()
        {
            var builder = new StringBuilder();
            builder.Append($"Parsed selection as '{mainType}'");
            if (mainType == MainType.All) return builder.Append('.').ToString();
            var tokensString = $" with tokens [{string.Join(", ", tokens)}]";
            if (mainType == MainType.Verse || mainType == MainType.Chapter) return builder.Append(tokensString).Append('.').ToString();
            var rangeString = $" of '{rangeType}'";
            return builder.Append(rangeString).Append(tokensString).Append('.').ToString();
        }

        public static bool TryParse(string value, out VerseSelection selection)
        {
            selection = null;
            if (!TryGetTokens(value.Trim(), out var mainType, out var rangeType, out var tokens)) return false;
            selection = new
            (
                mainType,
                rangeType,
                tokens
            );
            return true;
        }

        protected enum MainType : byte
        {
            All, // "all"
            Chapter, // <chapter>
            Verse, // <chapter>:<verse>
            Range // <position>:<position>
        }

        protected enum RangeType : byte
        {
            None,
            ChapterFromStart, // ..<chapter>
            VerseFromStart, // ..<chapter>:<verse>
            ChapterToEnd, // <chapter>..
            VerseToEnd, // <chapter>:<verse>..
            LeftRange, // <chapter>..<chapter>:<verse>
            ChapterToVerse, // <chapter>:..<verse>
            RightRange, // <chapter>:<verse>..<verse>
            ChapterToChapter, // <chapter>..<chapter>
            VerseToVerse // <chapter>:<verse>..<chapter>:<verse>
        }

        protected readonly MainType mainType;
        protected readonly RangeType rangeType;
        protected readonly string[] tokens;

        public int VerseId1 { get; private set; }
        public int VerseId2 { get; private set; }

        protected static bool TryGetTokens(string value, out MainType mainType, out RangeType rangeType, out string[] tokens)
        {
            tokens = default;
            mainType = default;
            rangeType = default;
            var splitArity = Splitter.GetSplit(value, "..", out var split);
            if (splitArity == Splitter.Arity.One)
            {
                splitArity = Splitter.GetSplit(split.First, ":", out split);
                if (splitArity == Splitter.Arity.One)
                {
                    if (split.First.ToLower().Equals("all"))
                    {
                        mainType = MainType.All;
                        return true;
                    }
                    if (split.First.IsChapterIdentifier())
                    {
                        mainType = MainType.Chapter;
                        tokens = [split.First];
                        return true;
                    }
                }
                else if (splitArity == Splitter.Arity.Two && split.First.IsChapterIdentifier() && split.Last.IsNumeric())
                {
                    mainType = MainType.Verse;
                    tokens = [split.First, split.Last];
                    return true;
                }
            }
            else if (splitArity == Splitter.Arity.Two) return TryGetTokens(split, out mainType, out rangeType, out tokens);
            return false;
        }

        protected static bool TryGetTokens((string First, string Last) split, out MainType mainType, out RangeType rangeType, out string[] tokens)
        {
            tokens = default;
            rangeType = default;
            mainType = MainType.Range;
            var splitArity1 = Splitter.GetSplit(split.First, ":", out var split1);
            var splitArity2 = Splitter.GetSplit(split.Last, ":", out var split2);
            if (splitArity1 == Splitter.Arity.Empty)
            {
                if (splitArity2 == Splitter.Arity.One)
                {
                    // ..<chapter>
                    if (split2.First.IsChapterIdentifier())
                    {
                        tokens = [split2.First];
                        rangeType = RangeType.ChapterFromStart;
                        return true;
                    }
                }
                else if (splitArity2 == Splitter.Arity.Two)
                {
                    // ..<chapter>:<verse>
                    if (split2.First.IsChapterIdentifier() && split2.Last.IsNumeric())
                    {
                        tokens = [split2.First, split2.Last];
                        rangeType = RangeType.VerseFromStart;
                        return true;
                    }
                }
            }
            else if (splitArity2 == Splitter.Arity.Empty)
            {
                if (splitArity1 == Splitter.Arity.One)
                {
                    // <chapter>..
                    if (split1.First.IsChapterIdentifier())
                    {
                        tokens = [split1.First];
                        rangeType = RangeType.ChapterToEnd;
                        return true;
                    }
                }
                else if (splitArity1 == Splitter.Arity.Two)
                {
                    // <chapter>:<verse>..
                    if (split1.First.IsChapterIdentifier() && split1.Last.IsNumeric())
                    {
                        tokens = [split1.First, split1.Last];
                        rangeType = RangeType.VerseToEnd;
                        return true;
                    }
                }
            }
            else if (splitArity1 == Splitter.Arity.One && splitArity2 == Splitter.Arity.Two)
            {
                // <chapter>..<chapter>:<verse>
                if (split1.First.IsChapterIdentifier() && split2.First.IsChapterIdentifier() && split2.Last.IsNumeric())
                {
                    tokens = [split1.First, split2.First, split2.Last];
                    rangeType = RangeType.LeftRange;
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.Two && splitArity2 == Splitter.Arity.One)
            {
                // <chapter>:<verse>..<verse>
                if (split1.First.IsChapterIdentifier() && split1.Last.IsNumeric() && split2.First.IsNumeric())
                {
                    tokens = [split1.First, split1.Last, split2.First];
                    rangeType = RangeType.RightRange;
                    return true;
                }
                // <chapter>:..<verse>
                if (split1.First.IsChapterIdentifier() && split1.Last.Length == 0 && split2.First.IsNumeric())
                {
                    tokens = [split1.First, split2.First];
                    rangeType = RangeType.ChapterToVerse;
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.One && splitArity2 == Splitter.Arity.One)
            {
                // <chapter>..<chapter>
                if (split1.First.IsChapterIdentifier() && split2.First.IsChapterIdentifier())
                {
                    tokens = [split1.First, split2.First];
                    rangeType = RangeType.ChapterToChapter;
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.Two && splitArity2 == Splitter.Arity.Two)
            {
                // <chapter>:<verse>..<chapter>:<verse>
                if (split1.First.IsChapterIdentifier() && split1.Last.IsNumeric() && split2.First.IsChapterIdentifier() && split2.Last.IsNumeric())
                {
                    tokens = [split1.First, split1.Last, split2.First, split2.Last];
                    rangeType = RangeType.VerseToVerse;
                    return true;
                }
            }
            return false;
        }
    }
}