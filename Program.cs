using System;
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

            // #region verse
            var ayatSelectionArgument = new Argument<AyatSelection>("selection", AyatSelection.ArgumentParse, false, "A selection of verses from the Quran.");
            var indexOption = new Option<bool>(["--index", "-i"], "Display indexes above every word in each verse.");
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
            var surahsSelectionArgument = new Argument<SurahSelection>("selection", SurahSelection.ArgumentParse, false, "A selection of chapters from the Quran.");
            var chapterCommand = new Command("chapter", "Output information for a chapter or range of chapters from the Quran.")
            {
                surahsSelectionArgument
            };
            chapterCommand.AddAlias("surah");
            chapterCommand.SetHandler(ChapterHandler.Handle, surahsSelectionArgument);
            // #endregion

            // #region note
            var textArgument = new Argument<string>("text", "The text to include in the message")
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var selectionOrGroupArgument = new Argument<AyatSelectionOrGroup>("selection|group", AyatSelectionOrGroup.ArgumentParse, false, "TODO: add description...")
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var noteCommand = new Command("note", "TODO: add description...")
            {
                ayatSelectionArgument, // TODO: use a different argument for this to exclude indexes
                textArgument,
                selectionOrGroupArgument
            };
            noteCommand.SetHandler(NoteHandler.Handle, ayatSelectionArgument, textArgument, selectionOrGroupArgument);
            // #endregion

            // #region build-db
            var buildDatabaseCommand = new Command("build-db", "Download the resource files and rebuild the SQLite database.");
            buildDatabaseCommand.SetHandler(BuildDatabaseHandler.Handle);
            // #endregion

            var rootCommand = new RootCommand($"The {Defaults.applicationName} is a...")
            {
                verseCommand,
                chapterCommand,
                noteCommand,
                buildDatabaseCommand
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