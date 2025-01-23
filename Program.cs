using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using QuranCli.Arguments;
using QuranCli.Commands;
using QuranCli.Data;
using QuranCli.Utilities;

namespace QuranCli
{
    public static partial class Program
    {
        public static readonly string version = "1.0.0";
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
            verseCommand.AddAlias("vs");
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
            chapterCommand.AddAlias("ch");
            chapterCommand.SetHandler(ChapterHandler.Handle, chaptersSelectionArgument);
            // #endregion

            // #region search
            var queryArgument = new Argument<string[]>("query", "The search terms.");
            var limitOption = new Option<int>(["--limit", "-l"], @$"The maximum amount of results to display.
Should be between {Defaults.searchResultLimit.min} and {Defaults.searchResultLimit.max}."
            );
            limitOption.SetDefaultValue(3);
            var searchCommand = new Command("search", "Search for a verse from the Quran.")
            {
                queryArgument,
                limitOption
            };
            searchCommand.AddAlias("s");
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
            var listByOption = new Option<ListByOption>("--by", "Filter notes by tags, macros, or both");
            listByOption.SetDefaultValue(ListByOption.Macro);
            var listNoteCommand = new Command("list", "List notes filtered by a selection or by an ID.")
            {
                noteSelectionArgument,
                listByOption
            };
            listNoteCommand.AddAlias("ls");
            listNoteCommand.SetHandler(ListNoteHandler.Handle, noteSelectionArgument, listByOption);
            // #endregion

            // #region note id
            var idsArgument = new Argument<int[]>("id", "The ID of a note.")
            {
                Arity = ArgumentArity.OneOrMore
            };
            var getNoteCommand = new Command("get", "Get a note by its ID")
            {
                idsArgument
            };
            getNoteCommand.SetHandler(GetNoteHandler.Handle, idsArgument);
            // #endregion

            // #region note add
            var messageOption = new Option<string>(["--message", "-m"], "The message content of the note.");
            var forceOption = new Option<bool>(["--force", "-f"], "Force the note when it has no references included.");
            var addNoteCommand = new Command("add", "Add a new from the command line argument, standard input, or from your default text editor.")
            {
                messageOption,
                forceOption
            };
            addNoteCommand.AddAlias("+");
            addNoteCommand.SetHandler(AddNoteHandler.Handle, messageOption, forceOption);
            // #endregion

            // #region note edit
            var idArgument = new Argument<int>("id", "The ID of a note.");
            var editNoteCommand = new Command("edit", "Edit an existing note.")
            {
                idArgument,
                messageOption,
                forceOption
            };
            editNoteCommand.AddAlias("e");
            editNoteCommand.SetHandler(EditNoteHandler.Handle, idArgument, messageOption, forceOption);
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
                idsArgument
            };
            removeNoteCommand.AddAlias("rm");
            removeNoteCommand.AddAlias("-");
            removeNoteCommand.SetHandler(RemoveNoteHandler.Handle, idsArgument);
            // #endregion

            // #region note
            var noteCommand = new Command("note", "Subcommands for managing notes.")
            {
                listNoteCommand,
                getNoteCommand,
                addNoteCommand,
                editNoteCommand,
                getReferencesCommand,
                removeNoteCommand
            };
            noteCommand.AddAlias("nt");
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
                buildDatabaseCommand
            };

            rootCommand.AddGlobalOption(verboseOption);
            var cli = new CommandLineBuilder(rootCommand)
                .AddMiddleware(InitializeHandler)
                .UseHelp()
                .AddMiddleware(VersionHandler)
                .UseParseErrorReporting()
                .UseExceptionHandler(ExceptionHandler)
                .AddMiddleware(FinalizeExecution)
                .Build();
            return cli.Invoke(args);
        }

        private static Task InitializeHandler(InvocationContext context, Func<InvocationContext, Task> next)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;
            Directory.CreateDirectory(Defaults.configurationPath);
            if (!File.Exists(Defaults.databasePath))
            {
                var client = new HttpClient();
                client.Download($"{Defaults.resourceUrl}/{Defaults.databaseFileName}", Defaults.databasePath);
            }
            Logger.Verbose = (bool)context.ParseResult.GetValueForOption(verboseOption);
            return next(context);
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

        private static Task VersionHandler(InvocationContext context, Func<InvocationContext, Task> next)
        {
            var tokens = context.ParseResult.Tokens;
            if (tokens.Count != 1) return next(context);
            var firstToken = tokens[0].ToString();
            if (firstToken == "-v" || firstToken == "--version" || firstToken == "version")
            {
                Logger.Message(version);
                return Task.CompletedTask;
            }
            return next(context);
        }
    }
}
