using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    internal partial class AyatSelection
    {
        protected AyatSelection
        (
            MainType mainType,
            RangeType rangeType,
            string[] tokens
        )
        {
            this.mainType = mainType;
            this.rangeType = rangeType;
            this.tokens = tokens;
        }

        public virtual string GetLog()
        {
            var builder = new StringBuilder();
            builder.Append($"Parsed selection as '{mainType}'");
            if (mainType == MainType.All) return builder.Append('.').ToString();
            var tokensString = $" with tokens [{string.Join(", ", tokens)}]";
            if (mainType == MainType.Ayah || mainType == MainType.Surah) return builder.Append(tokensString).Append('.').ToString();
            var rangeString = $" of '{rangeType}'";
            return builder.Append(rangeString).Append(tokensString).Append('.').ToString();
        }

        public static bool TryParse(string value, out AyatSelection selection)
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
            Surah, // <surah>
            Ayah, // <surah>:<ayah>
            Range // <position>:<position>
        }

        protected enum RangeType : byte
        {
            None,
            SurahFromStart, // ..<surah>
            AyahFromStart, // ..<surah>:<ayah>
            SurahToEnd, // <surah>..
            AyahToEnd, // <surah>:<ayah>..
            LeftRange, // <surah>..<surah>:<ayah>
            SurahToAyah, // <surah>:..<ayah>
            RightRange, // <surah>:<ayah>..<ayah>
            SurahToSurah, // <surah>..<surah>
            AyahToAyah // <surah>:<ayah>..<surah>:<ayah>
        }

        protected readonly MainType mainType;
        protected readonly RangeType rangeType;
        protected readonly string[] tokens;

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
                    if (split.First.IsSurahIdentifier())
                    {
                        mainType = MainType.Surah;
                        tokens = [split.First];
                        return true;
                    }
                }
                else if (splitArity == Splitter.Arity.Two && split.First.IsSurahIdentifier() && split.Last.IsNumeric())
                {
                    mainType = MainType.Ayah;
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
                    // ..<surah>
                    if (split2.First.IsSurahIdentifier())
                    {
                        tokens = [split2.First];
                        rangeType = RangeType.SurahFromStart;
                        return true;
                    }
                }
                else if (splitArity2 == Splitter.Arity.Two)
                {
                    // ..<surah>:<ayah>
                    if (split2.First.IsSurahIdentifier() && split2.Last.IsNumeric())
                    {
                        tokens = [split2.First, split2.Last];
                        rangeType = RangeType.AyahFromStart;
                        return true;
                    }
                }
            }
            else if (splitArity2 == Splitter.Arity.Empty)
            {
                if (splitArity1 == Splitter.Arity.One)
                {
                    // <surah>..
                    if (split1.First.IsSurahIdentifier())
                    {
                        tokens = [split1.First];
                        rangeType = RangeType.SurahToEnd;
                        return true;
                    }
                }
                else if (splitArity1 == Splitter.Arity.Two)
                {
                    // <surah>:<ayah>..
                    if (split1.First.IsSurahIdentifier() && split1.Last.IsNumeric())
                    {
                        tokens = [split1.First, split1.Last];
                        rangeType = RangeType.AyahToEnd;
                        return true;
                    }
                }
            }
            else if (splitArity1 == Splitter.Arity.One && splitArity2 == Splitter.Arity.Two)
            {
                // <surah>..<surah>:<ayah>
                if (split1.First.IsSurahIdentifier() && split2.First.IsSurahIdentifier() && split2.Last.IsNumeric())
                {
                    tokens = [split1.First, split2.First, split2.Last];
                    rangeType = RangeType.LeftRange;
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.Two && splitArity2 == Splitter.Arity.One)
            {
                // <surah>:<ayah>..<ayah>
                if (split1.First.IsSurahIdentifier() && split1.Last.IsNumeric() && split2.First.IsNumeric())
                {
                    tokens = [split1.First, split1.Last, split2.First];
                    rangeType = RangeType.RightRange;
                    return true;
                }
                // <surah>:..<ayah>
                if (split1.First.IsSurahIdentifier() && split1.Last.Length == 0 && split2.First.IsNumeric())
                {
                    tokens = [split1.First, split2.First];
                    rangeType = RangeType.SurahToAyah;
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.One && splitArity2 == Splitter.Arity.One)
            {
                // <surah>..<surah>
                if (split1.First.IsSurahIdentifier() && split2.First.IsSurahIdentifier())
                {
                    tokens = [split1.First, split2.First];
                    rangeType = RangeType.SurahToSurah;
                    return true;
                }
            }
            else if (splitArity1 == Splitter.Arity.Two && splitArity2 == Splitter.Arity.Two)
            {
                // <surah>:<ayah>..<surah>:<ayah>
                if (split1.First.IsSurahIdentifier() && split1.Last.IsNumeric() && split2.First.IsSurahIdentifier() && split2.Last.IsNumeric())
                {
                    tokens = [split1.First, split1.Last, split2.First, split2.Last];
                    rangeType = RangeType.AyahToAyah;
                    return true;
                }
            }
            return false;
        }
    }
}