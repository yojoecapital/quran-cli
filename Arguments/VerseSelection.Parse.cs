using System;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class VerseSelection
    {
        public int ChapterNumber1 { get; private set; }
        public int ChapterNumber2 { get; private set; }
        public bool IsChapterSelection { get; private set; }

        public static bool TryParse(string value, out VerseSelection selection)
        {
            selection = new();
            return selection.TryGetVerseIds(value.Trim().ToLower());
        }

        public int VerseId1 { get; private set; }
        public int VerseId2 { get; private set; }

        protected bool TryGetVerseIds(string value)
        {
            var splitArity = Splitter.GetSplit(value, "..", out var split);
            if (splitArity == Splitter.Arity.One)
            {
                splitArity = Splitter.GetSplit(split.First, ":", out split);
                if (splitArity == Splitter.Arity.One)
                {
                    if (split.First.Equals("all"))
                    {
                        SetChapterNumbers(1, 114);
                        VerseId1 = 1;
                        VerseId2 = 6236;
                        return true;
                    }
                    if (split.First.IsPageOrJuzIdentifier())
                    {
                        var (verseId1, verseId2) = SelectionHelpers.GetVerseIdsByPageOrJuzIdentifier(split.First);
                        VerseId1 = verseId1;
                        VerseId2 = verseId2;
                        return true;
                    }
                    if (split.First.IsChapterIdentifier())
                    {
                        var chapter = SelectionHelpers.GetChapterByIdentifier(split.First);
                        SetChapterNumbers(chapter.Number, chapter.Number);
                        VerseId1 = chapter.Start;
                        VerseId2 = chapter.End;
                        return true;
                    }
                }
                else if (splitArity == Splitter.Arity.Two && split.First.IsChapterIdentifier() && split.Last.IsNumeric())
                {
                    VerseId1 = VerseId2 = SelectionHelpers.GetVerseIdByNumbers(split.First, int.Parse(split.Last));
                    return true;
                }
            }
            else if (splitArity == Splitter.Arity.Two) return TryGetVerseIds(split);
            return false;
        }

        private void SetChapterNumbers(int chapterNumber1, int chapterNumber2)
        {
            IsChapterSelection = true;
            ChapterNumber1 = chapterNumber1;
            ChapterNumber2 = chapterNumber2;
        }

        protected bool TryGetVerseIds((string First, string Last) split)
        {
            var splitArity1 = Splitter.GetSplit(split.First, ":", out var split1);
            var splitArity2 = Splitter.GetSplit(split.Last, ":", out var split2);
            if (splitArity1 == Splitter.Arity.Empty)
            {
                if (splitArity2 == Splitter.Arity.One)
                {
                    // ..<page>
                    if (split2.First.IsPageOrJuzIdentifier())
                    {
                        VerseId1 = 1;
                        VerseId2 = SelectionHelpers.GetVerseIdsByPageOrJuzIdentifier(split2.First).verseId2;
                        return true;
                    }
                    // ..<chapter>
                    if (split2.First.IsChapterIdentifier())
                    {
                        var chapter = SelectionHelpers.GetChapterByIdentifier(split2.First);
                        SetChapterNumbers(1, chapter.Number);
                        VerseId1 = 1;
                        VerseId2 = chapter.End;
                        return true;
                    }
                }
                else if (splitArity2 == Splitter.Arity.Two)
                {
                    // ..<chapter>:<verse>
                    if (split2.First.IsChapterIdentifier() && split2.Last.IsNumeric())
                    {
                        VerseId1 = 1;
                        VerseId2 = SelectionHelpers.GetVerseIdByNumbers(split2.First, int.Parse(split2.Last));
                        return true;
                    }
                }
            }
            else if (splitArity2 == Splitter.Arity.Empty)
            {
                if (splitArity1 == Splitter.Arity.One)
                {
                    // <page>..
                    if (split1.First.IsPageOrJuzIdentifier())
                    {
                        VerseId1 = SelectionHelpers.GetVerseIdsByPageOrJuzIdentifier(split1.First).verseId1;
                        VerseId2 = 6236;
                        return true;
                    }
                    // <chapter>..
                    if (split1.First.IsChapterIdentifier())
                    {
                        var chapter = SelectionHelpers.GetChapterByIdentifier(split1.First);
                        SetChapterNumbers(chapter.Number, 114);
                        VerseId1 = chapter.Start;
                        VerseId2 = 6236;
                        return true;
                    }
                }
                else if (splitArity1 == Splitter.Arity.Two)
                {
                    // <chapter>:<verse>..
                    if (split1.First.IsChapterIdentifier() && split1.Last.IsNumeric())
                    {
                        VerseId1 = SelectionHelpers.GetVerseIdByNumbers(split1.First, int.Parse(split1.Last));
                        VerseId2 = 6236;
                        return true;
                    }
                }
            }
            else if (splitArity1 == Splitter.Arity.One && splitArity2 == Splitter.Arity.Two)
            {
                // <page>..<chapter>:<verse>
                if (split1.First.IsPageOrJuzIdentifier() && split2.First.IsChapterIdentifier() && split2.Last.IsNumeric())
                {
                    VerseId1 = SelectionHelpers.GetVerseIdsByPageOrJuzIdentifier(split1.First).verseId1;
                    VerseId2 = SelectionHelpers.GetVerseIdByNumbers(split2.First, int.Parse(split2.Last));
                    return true;
                }
                // <chapter>..<chapter>:<verse>
                if (split1.First.IsChapterIdentifier() && split2.First.IsChapterIdentifier() && split2.Last.IsNumeric())
                {
                    var chapter = SelectionHelpers.GetChapterByIdentifier(split1.First);
                    VerseId1 = chapter.Start;
                    VerseId2 = SelectionHelpers.GetVerseIdByNumbers(split2.First, int.Parse(split2.Last));
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.Two && splitArity2 == Splitter.Arity.One)
            {
                // <chapter>:<verse>..<page>
                if (split1.First.IsChapterIdentifier() && split1.Last.IsNumeric() && split2.First.IsPageOrJuzIdentifier())
                {
                    VerseId1 = SelectionHelpers.GetVerseIdByNumbers(split1.First, int.Parse(split1.Last));
                    VerseId2 = SelectionHelpers.GetVerseIdsByPageOrJuzIdentifier(split2.First).verseId2;
                    return true;
                }
                // <chapter>:<verse>..<verse>
                if (split1.First.IsChapterIdentifier() && split1.Last.IsNumeric() && split2.First.IsNumeric())
                {
                    var chapter = SelectionHelpers.GetChapterByIdentifier(split1.First);
                    VerseId1 = chapter.Start + int.Parse(split1.Last) - 1;
                    VerseId2 = chapter.Start + int.Parse(split2.First) - 1;
                    return true;
                }
                // <chapter>:..<verse>
                if (split1.First.IsChapterIdentifier() && split1.Last.Length == 0 && split2.First.IsNumeric())
                {
                    var chapter = SelectionHelpers.GetChapterByIdentifier(split1.First);
                    VerseId1 = chapter.Start;
                    VerseId2 = chapter.Start + int.Parse(split2.First) - 1;
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.One && splitArity2 == Splitter.Arity.One)
            {
                if (split1.First.IsPageOrJuzIdentifier() && split2.First.IsPageOrJuzIdentifier())
                {
                    VerseId1 = SelectionHelpers.GetVerseIdsByPageOrJuzIdentifier(split1.First).verseId1;
                    VerseId2 = SelectionHelpers.GetVerseIdsByPageOrJuzIdentifier(split2.First).verseId2;
                    return true;
                }
                // <chapter>..<chapter>
                if (split1.First.IsChapterIdentifier() && split2.First.IsChapterIdentifier())
                {
                    var chapter1 = SelectionHelpers.GetChapterByIdentifier(split1.First);
                    var chapter2 = SelectionHelpers.GetChapterByIdentifier(split2.First);
                    SetChapterNumbers(chapter1.Number, chapter2.Number);
                    VerseId1 = chapter1.Start;
                    VerseId2 = chapter2.End;
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.Two && splitArity2 == Splitter.Arity.Two)
            {
                // <chapter>:<verse>..<chapter>:<verse>
                if (split1.First.IsChapterIdentifier() && split1.Last.IsNumeric() && split2.First.IsChapterIdentifier() && split2.Last.IsNumeric())
                {
                    VerseId1 = SelectionHelpers.GetVerseIdByNumbers(split1.First, int.Parse(split1.Last));
                    VerseId2 = SelectionHelpers.GetVerseIdByNumbers(split2.First, int.Parse(split2.Last));
                    return true;
                }
            }
            return false;
        }
    }
}