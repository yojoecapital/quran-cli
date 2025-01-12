using System.CommandLine.Parsing;
using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    internal partial class QuranSelection
    {
        private QuranSelection
        (
            MainType mainType,
            RangeType rangeType,
            string[] tokens,
            bool isIndexed,
            int? from,
            int? to
        )
        {
            this.mainType = mainType;
            this.rangeType = rangeType;
            this.tokens = tokens;
            IsIndexed = isIndexed;
            this.from = from;
            this.to = to;
        }

        public bool IsIndexed { get; private set; }
        private readonly int? from;
        public int From => from.Value;
        public bool IsFromStart => !from.HasValue;
        private readonly int? to;
        public int To => to.Value;
        public bool IsToEnd => !to.HasValue;

        public void Log()
        {
            var builder = new StringBuilder();
            builder.Append($"Parsed selection as '{mainType}'");
            if (mainType == MainType.All)
            {
                Logger.Info(builder.Append('.').Append(GetIndexLog()));
                return;
            }
            var tokensString = $" with tokens [{string.Join(", ", tokens)}]";
            if (mainType == MainType.Ayah || mainType == MainType.Surah)
            {
                Logger.Info(builder.Append(tokensString).Append('.').Append(GetIndexLog()));
                return;
            }
            var rangeString = $" of '{rangeType}'";
            Logger.Info(builder.Append(rangeString).Append(tokensString).Append('.').Append(GetIndexLog()));
        }

        private string GetIndexLog()
        {
            if (!IsIndexed) return string.Empty;
            var builder = new StringBuilder(" Indexed from ");
            if (IsFromStart) builder.Append("start ");
            else builder.Append($"'{From}' ");
            builder.Append("to ");
            if (IsToEnd) builder.Append("end.");
            else builder.Append($"'{To}'.");
            return builder.ToString();
        }

        public static QuranSelection ArgumentParse(ArgumentResult result)
        {
            var validSelections = @"Valid selections include:  
  - <surah>
  - <surah>:<ayah>
  - <surah>:<ayah>..<surah>:<ayah>
  - <surah>:<ayah>..<ayah>
  - <surah>..<surah>:<ayah>
  - all";
            if (result.Tokens.Count == 0)
            {
                result.ErrorMessage = $"Selection argument was not provided. {validSelections}";
                return null;
            }
            if (result.Tokens.Count > 0 && TryParse(result.Tokens[0].ToString(), out var selection))
            {
                result.OnlyTake(1);
                return selection;
            }
            result.ErrorMessage = $"Could not parse selection. {validSelections}";
            return null;
        }

        public static bool TryParse(string value, out QuranSelection selection)
        {
            selection = null;
            var splitArity = GetSplit(value.Trim(), "_", out var split);
            int? from = null, to = null;
            bool isIndexed = false;
            if (!TryGetTokens(split.First, out var mainType, out var rangeType, out var tokens)) return false;
            if (splitArity == SplitArity.Two)
            {
                if (!TryGetIndexRange(split.Last, out from, out to)) return false;
                isIndexed = true;
            }
            selection = new
            (
                mainType,
                rangeType,
                tokens,
                isIndexed,
                from,
                to
            );
            return true;
        }

        private enum MainType : byte
        {
            All, // "all"
            Surah, // <surah>
            Ayah, // <surah>:<ayah>
            Range // <position>:<position>
        }

        private enum SplitArity : byte
        {
            Empty,
            One, // <value>
            Two // <value>:<value>
        }

        private enum RangeType : byte
        {
            None,
            SurahFromStart, // ..<surah>
            AyahFromStart, // ..<surah>:<ayah>
            SurahToEnd, // <surah>..
            AyahToEnd, // <surah>:<ayah>..
            LeftRange, // <surah>..<surah>:<ayah>
            RightRange, // <surah>:<ayah>..<ayah>
            SurahToSurah, // <surah>..<surah>
            AyahToAyah // <surah>:<ayah>..<surah>:<ayah>
        }

        private readonly MainType mainType;
        private readonly RangeType rangeType;
        private readonly string[] tokens;

        private static bool TryGetTokens(string value, out MainType mainType, out RangeType rangeType, out string[] tokens)
        {
            tokens = default;
            mainType = default;
            rangeType = default;
            var splitArity = GetSplit(value, "..", out var split);
            if (splitArity == SplitArity.One)
            {
                splitArity = GetSplit(split.First, ":", out split);
                if (splitArity == SplitArity.One)
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
                else if (splitArity == SplitArity.Two && split.First.IsSurahIdentifier() && split.Last.IsNumeric())
                {
                    mainType = MainType.Ayah;
                    tokens = [split.First, split.Last];
                    return true;
                }
            }
            else if (splitArity == SplitArity.Two) return TryGetTokens(split, out mainType, out rangeType, out tokens);
            return false;
        }

        private static bool TryGetTokens((string First, string Last) split, out MainType mainType, out RangeType rangeType, out string[] tokens)
        {
            tokens = default;
            rangeType = default;
            mainType = MainType.Range;
            var splitArity1 = GetSplit(split.First, ":", out var split1);
            var splitArity2 = GetSplit(split.Last, ":", out var split2);
            if (splitArity1 == SplitArity.Empty)
            {
                if (splitArity2 == SplitArity.One)
                {
                    // ..<surah>
                    if (split2.First.IsSurahIdentifier())
                    {
                        tokens = [split2.First];
                        rangeType = RangeType.SurahFromStart;
                        return true;
                    }
                }
                else if (splitArity2 == SplitArity.Two)
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
            else if (splitArity2 == SplitArity.Empty)
            {
                if (splitArity1 == SplitArity.One)
                {
                    // <surah>..
                    if (split1.First.IsSurahIdentifier())
                    {
                        tokens = [split1.First];
                        rangeType = RangeType.SurahToEnd;
                        return true;
                    }
                }
                else if (splitArity1 == SplitArity.Two)
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
            else if (splitArity1 == SplitArity.One && splitArity2 == SplitArity.Two)
            {
                // <surah>..<surah>:<ayah>
                if (split1.First.IsSurahIdentifier() && split2.First.IsSurahIdentifier() && split2.Last.IsNumeric())
                {
                    tokens = [split1.First, split2.First, split2.Last];
                    rangeType = RangeType.LeftRange;
                    return true;
                }
            }
            else if (splitArity1 == SplitArity.Two && splitArity2 == SplitArity.One)
            {
                // <surah>:<ayah>..<ayah>
                if (split1.First.IsSurahIdentifier() && split1.Last.IsNumeric() && split2.First.IsNumeric())
                {
                    tokens = [split1.First, split1.Last, split2.First];
                    rangeType = RangeType.RightRange;
                    return true;
                }
            }
            else if (splitArity1 == SplitArity.One && splitArity2 == SplitArity.One)
            {
                // <surah>:<ayah>..<surah>:<ayah>
                if (split1.First.IsSurahIdentifier() && split2.First.IsSurahIdentifier())
                {
                    tokens = [split1.First, split2.First];
                    rangeType = RangeType.SurahToSurah;
                    return true;
                }
            }
            else if (splitArity1 == SplitArity.Two && splitArity2 == SplitArity.Two)
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

        private static SplitArity GetSplit(string value, string seperator, out (string First, string Last) split)
        {
            split = default;
            if (value.Length == 0) return SplitArity.Empty;
            var index = value.IndexOf(seperator);
            if (index != -1)
            {
                split.First = value[..index].TrimEnd();
                split.Last = value[(index + seperator.Length)..].TrimStart();
                return SplitArity.Two;
            }
            split.First = split.Last = value;
            return SplitArity.One;
        }

        private static bool TryGetIndexRange(string value, out int? from, out int? to)
        {
            from = to = null;
            var splitArity = GetSplit(value, "..", out var split);
            if (splitArity == SplitArity.One)
            {
                // [<index>]
                if (int.TryParse(split.First, out var index))
                {
                    from = to = index;
                    return true;
                }
            }
            else if (splitArity == SplitArity.Two)
            {
                if (split.First.Length == 0)
                {
                    // [..<to>]
                    if (int.TryParse(split.Last, out var toValue))
                    {
                        to = toValue;
                        return true;
                    }
                }
                else if (split.Last.Length == 0)
                {
                    // [<from>..]
                    if (int.TryParse(split.First, out var fromValue))
                    {
                        from = fromValue;
                        return true;
                    }
                }
                else if (int.TryParse(split.First, out var fromValue) && int.TryParse(split.Last, out var toValue))
                {
                    // [<from>..<to>]
                    to = toValue;
                    from = fromValue;
                    return true;
                }
            }
            return false;
        }
    }
}