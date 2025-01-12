﻿using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using QuranCli.Arguments;
using QuranCli.Commands;
using QuranCli.Data;
using QuranCli.Utilities;

namespace QuranCli
{
    internal static partial class Program
    {
        public static Repository Repository { get; private set; }

        [STAThread]
        public static int Main(string[] args)
        {
            Initialize();

            // #region get
            var selectionArgument = new Argument<QuranSelection>("selection", QuranSelection.ArgumentParse, true, "A selection from the Quran.");
            var indexOption = new Option<bool>(["--index", "-i"], "Display indexes above every word in each verse.");
            var translationOption = new Option<bool>(["--translation", "-t"], "Include the translation in the output.");
            var numberOption = new Option<bool>(["--number", "-n"], "Include the verse number alongside each verse.");
            var verseCommand = new Command("verse", "Output a verse or range of verses from the Quran.")
            {
                selectionArgument,
                indexOption,
                numberOption,
                translationOption
            };
            verseCommand.SetHandler(VerseHandler.Handle, selectionArgument, indexOption, translationOption, numberOption);
            // #endregion

            // #region recompile-db
            var recompileDatabaseCommand = new Command("recompile-db", "Download the resource files and rebuild the SQLite database.");
            recompileDatabaseCommand.SetHandler(RecompileDatabaseHandler.Handle);
            // #endregion
            var rootCommand = new RootCommand($"The {Defaults.applicationName} is a...")
            {
                verseCommand,
                recompileDatabaseCommand
            };
            return rootCommand.Invoke(args);
        }

        private static void Initialize()
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;
            Directory.CreateDirectory(Defaults.configurationPath);
            if (!File.Exists(Defaults.databasePath))
            {
                var client = new HttpClient();
                client.Download($"{Defaults.resourceUrl}/{Defaults.databaseFileName}", Defaults.databasePath);
            }
        }
    }
}