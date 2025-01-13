using System.CommandLine.Parsing;
using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    internal partial class SurahSelection
    {
        private SurahSelection(Type type, string[] tokens)
        {
            this.type = type;
            this.tokens = tokens;
        }

        private enum Type : byte
        {
            All, // 'all'
            Surah, // <surah>
            SurahFromStart, // ..<surah>
            SurahToEnd, // <surah>..
            SurahToSurah // <surah>..<surah>
        }

        private Type type;
        private string[] tokens;

        public void Log()
        {
            var builder = new StringBuilder();
            builder.Append($"Parsed selection as '{type}'");
            if (type == Type.All)
            {
                Logger.Info(builder.Append('.'));
                return;
            }
            var tokensString = $" with tokens [{string.Join(", ", tokens)}]";
            Logger.Info(builder.Append(tokensString).Append('.'));
        }


        public static bool TryParse(string value, out SurahSelection selection)
        {
            selection = null;
            if (!TryGetTokens(value.Trim(), out var type, out var tokens)) return false;
            selection = new(type, tokens);
            return true;
        }

        private static bool TryGetTokens(string value, out Type type, out string[] tokens)
        {
            type = default;
            tokens = null;
            var splitArity = Splitter.GetSplit(value, "..", out var split);
            if (splitArity == SplitArity.One)
            {
                if (split.First.ToLower().Equals("all"))
                {
                    type = Type.All;
                    return true;
                }
                if (split.First.IsSurahIdentifier())
                {
                    type = Type.Surah;
                    tokens = [split.First];
                    return true;
                }
            }
            else if (splitArity == SplitArity.Two)
            {
                if (split.First.Length == 0 && split.Last.IsSurahIdentifier())
                {
                    type = Type.SurahFromStart;
                    tokens = [split.Last];
                    return true;
                }
                if (split.First.IsSurahIdentifier() && split.Last.Length == 0)
                {
                    type = Type.SurahToEnd;
                    tokens = [split.First];
                    return true;
                }
                if (split.First.IsSurahIdentifier() && split.Last.IsSurahIdentifier())
                {
                    type = Type.SurahToSurah;
                    tokens = [split.First, split.Last];
                    return true;
                }
            }
            return false;
        }

        public static SurahSelection ArgumentParse(ArgumentResult result)
        {
            var validSelections = @"Valid selections include:  
  - <surah>
  - <surah>..<surah>
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


    }
}