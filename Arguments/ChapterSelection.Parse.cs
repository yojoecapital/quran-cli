using System.CommandLine.Parsing;
using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class ChapterSelection
    {
        private ChapterSelection(Type type, string[] tokens)
        {
            this.type = type;
            this.tokens = tokens;
        }

        private enum Type : byte
        {
            All, // 'all'
            Chapter, // <chapter>
            ChapterFromStart, // ..<chapter>
            ChapterToEnd, // <chapter>..
            ChapterToChapter // <chapter>..<chapter>
        }

        private readonly Type type;
        private readonly string[] tokens;

        public string GetLog()
        {
            var builder = new StringBuilder();
            builder.Append($"Parsed selection as '{type}'");
            if (type == Type.All) return builder.Append('.').ToString();
            var tokensString = $" with tokens [{string.Join(", ", tokens)}]";
            return builder.Append(tokensString).Append('.').ToString();
        }


        public static bool TryParse(string value, out ChapterSelection selection)
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
            if (splitArity == Splitter.Arity.One)
            {
                if (split.First.ToLower().Equals("all"))
                {
                    type = Type.All;
                    return true;
                }
                if (split.First.IsChapterIdentifier())
                {
                    type = Type.Chapter;
                    tokens = [split.First];
                    return true;
                }
            }
            else if (splitArity == Splitter.Arity.Two)
            {
                if (split.First.Length == 0 && split.Last.IsChapterIdentifier())
                {
                    type = Type.ChapterFromStart;
                    tokens = [split.Last];
                    return true;
                }
                if (split.First.IsChapterIdentifier() && split.Last.Length == 0)
                {
                    type = Type.ChapterToEnd;
                    tokens = [split.First];
                    return true;
                }
                if (split.First.IsChapterIdentifier() && split.Last.IsChapterIdentifier())
                {
                    type = Type.ChapterToChapter;
                    tokens = [split.First, split.Last];
                    return true;
                }
            }
            return false;
        }

        public static ChapterSelection ArgumentParse(ArgumentResult result)
        {
            var validSelections = @"Valid selections include:  
  - <chapter>
  - <chapter>..<chapter>
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