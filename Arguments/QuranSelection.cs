using System;
using System.CommandLine.Parsing;
using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    internal class QuranSelection
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
            if (mainType == MainType.All)
            {
                Logger.Info($"Parsed selection as '{mainType}'.");
                return;
            }
            else if (mainType == MainType.Ayah || mainType == MainType.Surah)
            {
                Logger.Info($"Parsed selection as '{mainType}' with tokens [{string.Join(", ", tokens)}].");
                return;
            }
            Logger.Info($"Parsed selection as '{mainType}' of '{rangeType}' with tokens [{string.Join(", ", tokens)}].");
        }

        public static QuranSelection ArgumentParse(ArgumentResult result)
        {
            if (result.Tokens.Count > 0 && TryParse(result.Tokens[0].ToString(), out var selection))
            {
                result.OnlyTake(1);
                return selection;
            }
            result.ErrorMessage = @"Could not parse selection. Valid selections include:
  - <surah>
  - <surah>:<ayah>
  - <surah>:<ayah>..<surah>:<ayah>
  - <surah>:<ayah>..<ayah>
  - <surah>..<surah>:<ayah>
  - all";
            return null;
        }

        public static bool TryParse(string value, out QuranSelection selection)
        {
            selection = null;
            TryCheckIfIndexed(value.Trim(), out var positionPart, out var indexPart);
            int? from = null, to = null;
            bool isIndexed = false;
            if (indexPart != null)
            {
                if (!TryGetIndexRange(indexPart, out to, out from)) return false;
                isIndexed = true;
            }
            if (!TryGetTokens(positionPart, out var mainType, out var rangeType, out var tokens)) return false;
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

        private static bool TryCheckIfIndexed(string value, out string positionPart, out string indexPart)
        {
            positionPart = value;
            indexPart = null;

            // make sure we have at most 1 opening
            var indexOpening = value.IndexOf('[');
            if (indexOpening != value.LastIndexOf('[')) return false;

            // check for index
            if (indexOpening >= 0)
            {
                if (!value.EndsWith(']')) return false;
                indexPart = value.Substring(indexOpening + 1, value.Length - indexOpening - 1).Trim();
                positionPart = value[..indexOpening].TrimEnd();
            }
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

            // split on the spread operator
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
            else if (splitArity == SplitArity.Two)
            {
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

        private static bool TryGetIndexRange(string value, out int? to, out int? from)
        {
            to = from = null;
            {
                var spreader = value.IndexOf("..");
                if (spreader < 0)
                {
                    // [<index>]
                    if (int.TryParse(value, out var index))
                    {
                        from = to = index;
                        return true;
                    }
                    return false;
                }
                var indexLeftPart = value[..spreader].Trim();
                var indexRightPart = value[spreader..].Trim();
                if (indexLeftPart.Length == 0)
                {
                    // [..<to>]
                    if (int.TryParse(indexRightPart, out var toValue))
                    {
                        to = toValue;
                        return true;
                    }
                }
                else if (indexRightPart.Length == 0)
                {
                    // [<from>..]
                    if (int.TryParse(indexLeftPart, out var fromValue))
                    {
                        from = fromValue;
                        return true;
                    }
                }
                else
                {
                    // [<from>..<to>]
                    if (int.TryParse(indexLeftPart, out var fromValue) && int.TryParse(indexRightPart, out var toValue))
                    {
                        to = toValue;
                        from = fromValue;
                        return true;
                    }
                }
            }
            return true;
        }
    }
}