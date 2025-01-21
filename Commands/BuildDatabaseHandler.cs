using System.IO;
using System.Net.Http;
using System.Text;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    public static class BuildDatabaseHandler
    {
        private static readonly string chaptersFilePath = Path.Join(Defaults.temporaryPath, Defaults.chaptersFileName);
        private static readonly string translationsFilePath = Path.Join(Defaults.temporaryPath, Defaults.translationsFileName);
        private static readonly string versesFilePath = Path.Join(Defaults.temporaryPath, Defaults.versesFileName);

        public static void Handle()
        {
            File.Delete(Defaults.databasePath);
            Chapter.CreateTable();
            Verse.CreateTable();
            Directory.CreateDirectory(Defaults.temporaryPath);
#if !DEBUG
            DownloadFiles();
#endif
            Logger.Message("Seeding database. This may take a while.");
            ConsumeFiles();
            Logger.Message("Seeding complete.");
            Directory.Delete(Defaults.temporaryPath, true);
        }

        private static void DownloadFiles()
        {
            using var client = new HttpClient();
            client.Download($"{Defaults.resourceUrl}/{Defaults.chaptersFileName}", chaptersFilePath);
            client.Download($"{Defaults.resourceUrl}/{Defaults.translationsFileName}", translationsFilePath);
            client.Download($"{Defaults.resourceUrl}/{Defaults.versesFileName}", versesFilePath);
        }

        private static void ConsumeFiles()
        {
            // Consume the files
            using var chaptersReader = new StreamReader(chaptersFilePath, Encoding.UTF8);
            using var translationsReader = new StreamReader(translationsFilePath, Encoding.UTF8);
            using var versesReader = new StreamReader(versesFilePath, Encoding.UTF8);
            int verseId = 0, versesLeftInChapter = 0, chapterNumber = 0, totalVerses = 6236;
            Chapter chapter = null;
            string text, translation, chapterLine;
            while ((text = versesReader.ReadLine()) != null && (translation = translationsReader.ReadLine()) != null)
            {
                if (versesLeftInChapter == 0 && (chapterLine = chaptersReader.ReadLine()) != null)
                {
                    chapterNumber++;
                    chapter = GetChapter(chapterNumber, chapterLine);
                    versesLeftInChapter = chapter.Count;
                    chapter.Insert();
                }
                verseId++;
                versesLeftInChapter--;
                var verse = new Verse()
                {
                    Id = verseId,
                    Chapter = chapterNumber,
                    Number = chapter.Count - versesLeftInChapter,
                    Text = text,
                    Translation = translation
                };
                verse.Insert();
                Logger.Percent(verseId, totalVerses);
            }
        }

        private static Chapter GetChapter(int lineNumber, string line)
        {
            var parts = line.Split(',');
            return new Chapter()
            {
                Number = lineNumber,
                Count = int.Parse(parts[0]),
                Start = int.Parse(parts[1]),
                Name = parts[2],
                Transliteration = parts[3],
                Translation = parts[4]
            };
        }
    }
}