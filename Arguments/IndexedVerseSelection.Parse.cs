using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Arguments
{
    public partial class IndexedVerseSelection : VerseSelection
    {
        private IndexedVerseSelection
        (
            bool isIndexed,
            int? from,
            int? to
        )
        {
            IsIndexed = isIndexed;
            this.from = from;
            this.to = to;
        }

        private IndexedVerseSelection() { }

        public bool IsIndexed { get; private set; }
        private readonly int? from;
        public int From => from.Value;
        public bool IsFromStart => !from.HasValue;
        private readonly int? to;
        public int To => to.Value;
        public bool IsToEnd => !to.HasValue;

        public static bool TryParse(string value, out IndexedVerseSelection selection)
        {
            selection = null;
            var splitArity = Splitter.GetSplit(value.Trim(), "::", out var split);
            if (splitArity == Splitter.Arity.Two)
            {
                if (!TryGetIndexRange(split.Last, out var from, out var to)) return false;
                selection = new(true, from, to);
            }
            else selection = new();
            return selection.TryGetVerseIds(split.First);
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