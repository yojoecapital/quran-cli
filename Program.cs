using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using QuranCli.Arguments;

namespace QuranCli
{
    internal static partial class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            // Make sure the configuration directory exists
            Directory.CreateDirectory(Defaults.configurationPath);

            // #region get
            var selectionArgument = new Argument<QuranSelection>("selection", QuranSelection.ArgumentParse, true, "A selection from the Quran.");
            var verseCommand = new Command("verse", "Output a verse or range of verses from the Quran.")
            {
                selectionArgument
            };
            verseCommand.SetHandler(selection => selection.Log(), selectionArgument);
            // #endregion

            var rootCommand = new RootCommand($"The {Defaults.applicationName} is a...")
            {
                verseCommand
            };
            return rootCommand.Invoke(args);
        }
    }
}