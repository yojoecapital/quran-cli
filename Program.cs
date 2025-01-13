using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Net.Http;
using System.Text;
using QuranCli.Commands;
using QuranCli.Data;
using QuranCli.Utilities;

namespace QuranCli
{
    internal static partial class Program
    {
        private static readonly Option verboseOption = new Option<bool>("--verbose", "Output [INFO] level messages.");

        [STAThread]
        public static int Main(string[] args)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            // #region verse
            var ayatSelectionArgument = new Argument<string>("selection", @"A selection of verses from the Quran.
The selection can be in the form of '<surah>:<ayah>..<surah>:<ayah>'. 
You can pass '<surah>:<ayah>' for a single verse or '<surah>' for an entire chapter.
You can optionally include an subsection of a selection with '<selection>::<index>..<index>'.");
            var indexOption = new Option<bool>(["--index", "-i"], @"Display indexes above every word in each verse.
These can be used as indexes in the selection argument.");
            var translationOption = new Option<bool>(["--translation", "-t"], "Include the translation in the output.");
            var numberOption = new Option<bool>(["--number", "-n"], "Include the verse number alongside each verse.");
            var verseCommand = new Command("verse", "Output a verse or range of verses from the Quran.")
            {
                ayatSelectionArgument,
                indexOption,
                numberOption,
                translationOption
            };
            verseCommand.SetHandler(VerseHandler.Handle, ayatSelectionArgument, indexOption, translationOption, numberOption);
            verseCommand.AddAlias("ayah");
            // #endregion

            // #region chapter
            var getOption = new Option<SurahField?>(["--get", "-g"], @"Only get this attribute.
The attributes include 'number', 'count', 'name', 'translation', and 'transliteration'.");
            var surahsSelectionArgument = new Argument<string>("selection", @"A selection of chapters from the Quran.
The selection can be in the form of '<surah>..<surah>'.
You can pass '<surah>' for information on a single chapter.");
            var chapterCommand = new Command("chapter", "Output information for a chapter or range of chapters from the Quran.")
            {
                surahsSelectionArgument,
                getOption
            };
            chapterCommand.AddAlias("surah");
            chapterCommand.SetHandler(ChapterHandler.Handle, surahsSelectionArgument, getOption);
            // #endregion

            // #region build-db
            var buildDatabaseCommand = new Command("build-db", "Download the resource files and rebuild the SQLite database.");
            buildDatabaseCommand.SetHandler(BuildDatabaseHandler.Handle);
            // #endregion

            var rootCommand = new RootCommand($"The {Defaults.applicationName} is a...")
            {
                verseCommand,
                chapterCommand,
                buildDatabaseCommand
            };
            rootCommand.AddGlobalOption(verboseOption);
            var builder = new CommandLineBuilder(rootCommand);
            builder.UseDefaults().AddMiddleware(Initialize);
            return builder.Build().Invoke(args);
        }

        private static void Initialize(InvocationContext context)
        {
            Directory.CreateDirectory(Defaults.configurationPath);
            if (!File.Exists(Defaults.databasePath))
            {
                var client = new HttpClient();
                client.Download($"{Defaults.resourceUrl}/{Defaults.databaseFileName}", Defaults.databasePath);
            }
            Logger.verbose = (bool)context.ParseResult.GetValueForOption(verboseOption);
        }
    }
}