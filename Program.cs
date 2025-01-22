﻿using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using QuranCli.Commands;
using QuranCli.Data;
using QuranCli.Utilities;

namespace QuranCli
{
    public static partial class Program
    {
        public static readonly string version = "1.0.0-beta";
        private static readonly Option verboseOption = new Option<bool>("--verbose", "Output [INFO] level messages.");

        public static int Main(string[] args)
        {
            // #region verse
            var selectionArgument = new Argument<string>(
                "selection",
                @"A selection from the Quran.
The selection can be specified as '<chapter>:<verse>..<chapter>:<verse>'.
For a single verse, use '<chapter>:<verse>', or for an entire chapter, use '<chapter>'."
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
            verseCommand.AddAlias("verse");
            // #endregion

            // #region chapter
            var chaptersSelectionArgument = new Argument<string>(
                "selection",
                @"A selection of chapters from the Quran.
The selection can be specified as '<chapter>..<chapter>'. For a single chapter, use '<chapter>'."
            );
            var chapterCommand = new Command("chapter", "Output information for a chapter or range of chapters from the Quran.")
            {
                chaptersSelectionArgument
            };
            chapterCommand.AddAlias("chapter");
            chapterCommand.SetHandler(ChapterHandler.Handle, chaptersSelectionArgument);
            // #endregion

            // #region search
            var queryArgument = new Argument<string>("query", "The search term.");
            var limitOption = new Option<int>("--limit", @$"The maximum amount of results to display.
Should be between {Defaults.searchResultLimit.min} and {Defaults.searchResultLimit.max}."
            );
            limitOption.SetDefaultValue(3);
            var searchCommand = new Command("search", "Search for a verse from the Quran.")
            {
                queryArgument,
                limitOption
            };
            searchCommand.SetHandler(SearchHandler.Handle, queryArgument, limitOption);
            // #endregion

            // #region note ls
            var noteSelectionArgument = new Argument<string>(
                "selection",
                "A selection from the Quran to filter notes on."
            )
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var idOption = new Option<int?>("--id", "The ID of a note.");
            var listNoteCommand = new Command("ls", "List notes filtered by a selection or by an ID.")
            {
                noteSelectionArgument,
                idOption
            };
            listNoteCommand.SetHandler(ListNoteHandler.Handle, noteSelectionArgument, idOption);
            // #endregion

            // #region note add
            var textArgument = new Argument<string>("text", "The content of the note.")
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var addNoteCommand = new Command("add", "Add a new from the command line argument, standard input, or from your default text editor.")
            {
                textArgument
            };
            addNoteCommand.SetHandler(AddNoteHandler.Handle, textArgument);
            // #endregion

            // #region note edit
            var idArgument = new Argument<int>("id", "The ID of a note.");
            var editNoteCommand = new Command("edit", "Edit an existing note.")
            {
                idArgument,
                textArgument
            };
            editNoteCommand.SetHandler(EditNoteHandler.Handle, idArgument, textArgument);
            // #endregion

            // #region note ref
            var getReferencesCommand = new Command("references", "Output the verses referenced by a note.")
            {
                idArgument
            };
            getReferencesCommand.AddAlias("ref");
            getReferencesCommand.SetHandler(GetReferencesHandler.Handle, idArgument);
            // #endregion

            // #region note rm
            var removeNoteCommand = new Command("remove", "Remove a note.")
            {
                idArgument
            };
            removeNoteCommand.AddAlias("rm");
            removeNoteCommand.SetHandler(RemoveNoteHandler.Handle, idArgument);
            // #endregion

            // #region note
            var noteCommand = new Command("note", "Subcommands for managing notes.")
            {
                listNoteCommand,
                addNoteCommand,
                editNoteCommand,
                getReferencesCommand,
                removeNoteCommand
            };
            // #endregion

            // #region version
            var versionCommand = new Command("version", "Display the current version.");
            versionCommand.AddAlias("--version");
            versionCommand.AddAlias("-v");
            versionCommand.SetHandler(VersionHandler.Handle);
            // #endregion

            // #region build-db
            var buildDatabaseCommand = new Command("build-db", "Download the resource files and rebuild the SQLite database.");
            buildDatabaseCommand.SetHandler(BuildDatabaseHandler.Handle);
            // #endregion

            var rootCommand = new RootCommand($"The {Defaults.applicationName} is a tool to output and annotate verses from the Noble book.")
            {
                verseCommand,
                chapterCommand,
                searchCommand,
                noteCommand,
                versionCommand,
                buildDatabaseCommand
            };

            rootCommand.AddGlobalOption(verboseOption);
            var cli = new CommandLineBuilder(rootCommand)
                .UseHelp()
                .UseParseErrorReporting()
                .UseExceptionHandler(ExceptionHandler)
                .AddMiddleware(Initialize, MiddlewareOrder.Configuration)
                .AddMiddleware(FinalizeExecution)
                .Build();
            return cli.Invoke(args);
        }

        private static void Initialize(InvocationContext context)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;
            Directory.CreateDirectory(Defaults.configurationPath);
            if (!File.Exists(Defaults.databasePath))
            {
                var client = new HttpClient();
                client.Download($"{Defaults.resourceUrl}/{Defaults.databaseFileName}", Defaults.databasePath);
            }
            Logger.Verbose = (bool)context.ParseResult.GetValueForOption(verboseOption);
        }

        private static async Task FinalizeExecution(InvocationContext context, Func<InvocationContext, Task> next)
        {
            await next(context);
            ConnectionManager.Close();
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
