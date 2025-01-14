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
            var selectionArgument = new Argument<string>(
                "selection",
                @"A selection from the Quran.
The selection can be specified as '<surah>:<ayah>..<surah>:<ayah>'.
For a single verse, use '<surah>:<ayah>', or for an entire chapter, use '<surah>'."
            );
            var indexOption = new Option<bool>(
                ["--index", "-i"],
                @"Display indexes above each word in the verses.
These indexes can optionally be used to select a subsection from the selection argument in the format '<selection>::<index>..<index>'."
            );
            var translationOption = new Option<bool>(
                ["--translation", "-t"],
                "Include the translation of the verses in the output."
            );
            var numberOption = new Option<bool>(
                ["--number", "-n"],
                "Include the verse numbers alongside each verse."
            );
            var verseCommand = new Command(
                "verse",
                "Display a verse or range of verses from the Quran."
            )
{
    selectionArgument,
    indexOption,
    numberOption,
    translationOption
};

            verseCommand.SetHandler(VerseHandler.Handle, selectionArgument, indexOption, translationOption, numberOption);
            verseCommand.AddAlias("ayah");
            // #endregion

            // #region chapter
            var getOption = new Option<SurahField?>(
                ["--get", "-g"],
                @"Only get this attribute.
The attributes include 'number', 'count', 'name', 'translation', and 'transliteration'."
            );
            var surahsSelectionArgument = new Argument<string>(
                "selection",
                @"A selection of chapters from the Quran.
The selection can be specified as '<surah>..<surah>'. For a single chapter, use '<surah>'."
            );
            var chapterCommand = new Command("chapter", "Output information for a chapter or range of chapters from the Quran.")
            {
                surahsSelectionArgument,
                getOption
            };
            chapterCommand.AddAlias("surah");
            chapterCommand.SetHandler(ChapterHandler.Handle, surahsSelectionArgument, getOption);
            // #endregion

            // #region note
            var noteArgument = new Argument<string>("note", "Include a note to create or edit.")
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var noteCommand = new Command("note", "Output, create, or edit a note on a selection of verses or chapters.")
            {
                selectionArgument,
                noteArgument
            };
            noteCommand.SetHandler(NoteHandler.Handle, selectionArgument, noteArgument);
            // #endregion

            // #region link
            var selectionOrGroupArgument = new Argument<string>(
                "selection|group",
                "Either a selection or group name."
            );
            var noteOption = new Option<string>(["--note", "-n"], "Optionally include a note.");
            var linkCommand = new Command("link", "Output, create, or edit links between verses or groups.")
            {
                selectionArgument,
                selectionOrGroupArgument,
                noteOption
            };
            linkCommand.SetHandler(LinkHandler.Handle, selectionArgument, selectionOrGroupArgument, noteOption);
            // #endregion

            // #region group
            var groupArgument = new Argument<string>(
                "name",
                "A group name."
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var groupCommand = new Command("group", "Output, create, or edit groups.")
            {
                groupArgument,
                noteOption
            }; ;
            groupCommand.SetHandler(GroupHandler.Handle, groupArgument, noteOption);
            // #endregion

            // #region remove
            var idToRemoveArgument = new Argument<string[]>(
                "id(s)",
                "The ID of the notes, links, or groups to remove."
            );
            var removeCommand = new Command("remove", "Remove notes, links, or groups.")
            {
                idToRemoveArgument
            };
            removeCommand.AddAlias("delete");
            removeCommand.AddAlias("rm");
            removeCommand.SetHandler(RemoveHandler.Handle, idToRemoveArgument);
            // #endregion

            // #region build-db
            var buildDatabaseCommand = new Command("build-db", "Download the resource files and rebuild the SQLite database.");
            buildDatabaseCommand.SetHandler(BuildDatabaseHandler.Handle);
            // #endregion

            var rootCommand = new RootCommand($"The {Defaults.applicationName} is a tool to output and annotate verses from the Noble book.")
            {
                verseCommand,
                chapterCommand,
                noteCommand,
                linkCommand,
                groupCommand,
                removeCommand,
                buildDatabaseCommand
            };
            rootCommand.AddGlobalOption(verboseOption);
            var cli = new CommandLineBuilder(rootCommand)
                .UseHelp()
                .UseParseErrorReporting()
                .UseExceptionHandler(ExceptionHandler)
                .AddMiddleware(Initialize)
                .Build();
            return cli.Invoke(args);
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

        private static void ExceptionHandler(Exception ex, InvocationContext context)
        {
            Logger.Error(ex.Message);
#if DEBUG
            Console.WriteLine(ex.StackTrace);
#endif
            context.ExitCode = 1;
        }
    }
}