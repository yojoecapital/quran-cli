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
        private static readonly string version = "0.0.1-beta";
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

            // #region build-db
            var buildDatabaseCommand = new Command("build-db", "Download the resource files and rebuild the SQLite database.");
            buildDatabaseCommand.SetHandler(BuildDatabaseHandler.Handle);
            // #endregion

            var rootCommand = new RootCommand($"The {Defaults.applicationName} is a tool to output and annotate verses from the Noble book.")
            {
                verseCommand,
                chapterCommand,
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
            Logger.verbose = (bool)context.ParseResult.GetValueForOption(verboseOption);
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
