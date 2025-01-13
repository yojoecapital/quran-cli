namespace QuranCli.Utilities
{
    internal enum SplitArity : byte
    {
        Empty,
        One, // <value>
        Two // <value>:<value>
    }

    internal static class Splitter
    {
        public static SplitArity GetSplit(string value, string seperator, out (string First, string Last) split)
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
    }
}