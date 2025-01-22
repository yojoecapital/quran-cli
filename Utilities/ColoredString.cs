using System;

namespace QuranCli.Utilities
{
    public class ColoredString(string text, ConsoleColor? color)
    {
        public readonly string text = text;
        public readonly ConsoleColor? color = color;
    }
}