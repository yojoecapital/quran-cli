using System.IO;
using System.Net.Http;
using System.Text;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    internal static class DatabaseBuilder
    {
        private static readonly string surahsFilePath = Path.Join(Defaults.temporaryPath, Defaults.surahsFileName);
        private static readonly string translationsFilePath = Path.Join(Defaults.temporaryPath, Defaults.translationsFileName);
        private static readonly string ayatFilePath = Path.Join(Defaults.temporaryPath, Defaults.ayatFileName);

        public static void Build()
        {
            Repository.Instance.CreateTables();
            Directory.CreateDirectory(Defaults.temporaryPath);
#if !DEBUG
            DownloadFiles();
#endif
            Logger.Message("Seeding database. This may take a while.");
            ConsumeFiles();
            Logger.Message("Syncing FTS table...");
            Repository.Instance.PopulateAyahFts();
            Logger.Message("Seeding and FTS syncing complete.");
            Directory.Delete(Defaults.temporaryPath, true);
        }

        private static void DownloadFiles()
        {
            using var client = new HttpClient();
            client.Download($"{Defaults.resourceUrl}/{Defaults.surahsFileName}", surahsFilePath);
            client.Download($"{Defaults.resourceUrl}/{Defaults.translationsFileName}", translationsFilePath);
            client.Download($"{Defaults.resourceUrl}/{Defaults.ayatFileName}", ayatFilePath);
        }

        private static void ConsumeFiles()
        {
            // Consume the files
            using var surahsReader = new StreamReader(surahsFilePath, Encoding.UTF8);
            using var translationsReader = new StreamReader(translationsFilePath, Encoding.UTF8);
            using var versesReader = new StreamReader(ayatFilePath, Encoding.UTF8);
            int ayahId = 0, ayatLeftInSurah = 0, surahId = 0, totalAyat = 6236;
            Surah surah = null;
            string verse, translation, surahLine;
            while ((verse = versesReader.ReadLine()) != null && (translation = translationsReader.ReadLine()) != null)
            {
                if (ayatLeftInSurah == 0 && (surahLine = surahsReader.ReadLine()) != null)
                {
                    surahId++;
                    surah = GetSurah(surahId, surahLine);
                    ayatLeftInSurah = surah.AyahCount;
                    Repository.Instance.Create(surah);
                }
                ayahId++;
                ayatLeftInSurah--;
                var ayah = new Ayah()
                {
                    Id = ayahId,
                    SurahId = surahId,
                    AyahNumber = surah.AyahCount - ayatLeftInSurah,
                    Verse = verse,
                    Translation = translation
                };
                Repository.Instance.Create(ayah);
                Logger.Percent(ayahId, totalAyat);
            }
        }

        private static Surah GetSurah(int lineNumber, string line)
        {
            var parts = line.Split(',');
            return new Surah()
            {
                Id = lineNumber,
                AyahCount = int.Parse(parts[0]),
                StartAyahId = int.Parse(parts[1]),
                Name = parts[2],
                TransliterationName = parts[3],
                EnglishName = parts[4]
            };
        }
    }
}