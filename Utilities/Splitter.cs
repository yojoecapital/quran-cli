namespace QuranCli.Utilities
{
    public static class Splitter
    {
        public enum Arity : byte
        {
            Empty,
            One, // <value>
            Two // <value>:<value>
        }

        public static Arity GetSplit(string value, string seperator, out (string First, string Last) split)
        {
            split = default;
            if (value.Length == 0) return Arity.Empty;
            var index = value.IndexOf(seperator);
            if (index != -1)
            {
                split.First = value[..index].TrimEnd();
                split.Last = value[(index + seperator.Length)..].TrimStart();
                return Arity.Two;
            }
            split.First = split.Last = value;
            return Arity.One;
        }
    }
}