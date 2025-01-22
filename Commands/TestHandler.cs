using System;
using System.Collections.Generic;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    public static class TestHandler
    {
        public static void Handle()
        {
            Test();
        }

        public static void Test()
        {
            var text = @" # This is a header!
*Welcome* to **PLANET EARTH:1** `human`.

## What to know

`This is a test` to test **you.**
This is a great chapter: {1:1}.
This is `also` **a great** verse **{1:2::3}**.
{1}
{2}
{3..4}
{all}
These are hashtags #2:1 and #al-baqara:5..4  and #2:3..4";
            WriteColoredStrings(MarkdownProcessor.GetColoredStrings(text));
        }

        public static void WriteColoredStrings(IEnumerable<ColoredString> strings)
        {
            foreach (var coloredString in strings)
            {
                if (coloredString.color.HasValue) Console.ForegroundColor = coloredString.color.Value;
                Console.Write(coloredString.text);
                Console.ResetColor();
            }
        }
    }
}