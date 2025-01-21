using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class IndexedVerseSelection : VerseSelection
    {
        private IndexedVerseSelection
        (
            MainType mainType,
            RangeType rangeType,
            string[] tokens,
            bool isIndexed,
            int? from,
            int? to
        ) : base(mainType, rangeType, tokens)
        {
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

        public override string GetLog() => base.GetLog() + ' ' + GetIndexLog();

        private string GetIndexLog()
        {
            if (!IsIndexed) return string.Empty;
            var builder = new StringBuilder("Indexed from ");
            if (IsFromStart) builder.Append("start ");
            else builder.Append($"'{From}' ");
            builder.Append("to ");
            if (IsToEnd) builder.Append("end.");
            else builder.Append($"'{To}'.");
            return builder.ToString();
        }

        public static bool TryParse(string value, out IndexedVerseSelection selection)
        {
            selection = null;
            var splitArity = Splitter.GetSplit(value.Trim(), "::", out var split);
            int? from = null, to = null;
            bool isIndexed = false;
            if (!TryGetTokens(split.First, out var mainType, out var rangeType, out var tokens)) return false;
            if (splitArity == Splitter.Arity.Two)
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

        private static bool TryGetIndexRange(string value, out int? from, out int? to)
        {
            from = to = null;
            var splitArity = Splitter.GetSplit(value, "..", out var split);
            if (splitArity == Splitter.Arity.One)
            {
                // [<index>]
                if (uint.TryParse(split.First, out var index))
                {
                    from = to = (int?)index;
                    return true;
                }
            }
            else if (splitArity == Splitter.Arity.Two)
            {
                if (split.First.Length == 0)
                {
                    // [..<to>]
                    if (uint.TryParse(split.Last, out var toValue))
                    {
                        to = (int?)toValue;
                        return true;
                    }
                }
                else if (split.Last.Length == 0)
                {
                    // [<from>..]
                    if (uint.TryParse(split.First, out var fromValue))
                    {
                        from = (int?)fromValue;
                        return true;
                    }
                }
                else if (uint.TryParse(split.First, out var fromValue) && int.TryParse(split.Last, out var toValue))
                {
                    // [<from>..<to>]
                    to = toValue;
                    from = (int?)fromValue;
                    if (to < from) return false;
                    return true;
                }
            }
            return false;
        }
    }
}