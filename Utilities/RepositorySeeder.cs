using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.Data.Sqlite;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    internal static class RepositorySeeder
    {
        private static readonly string surahsFilePath = Path.Join(Defaults.temporaryPath, Defaults.surahsFileName);
        private static readonly string translationsFilePath = Path.Join(Defaults.temporaryPath, Defaults.translationsFileName);
        private static readonly string versesFilePath = Path.Join(Defaults.temporaryPath, Defaults.versesFileName);

        public static void Seed(this Repository repository)
        {
            Directory.CreateDirectory(Defaults.temporaryPath);
            // DownloadFiles();
            Logger.Message("Seeding database. This may take a while.");
            ConsumeFiles(repository);
            Logger.Message("Syncing FTS table...");
            PopulateAyahFts(repository.connection);
            Logger.Message("Seeding and FTS syncing complete.");
            Directory.Delete(Defaults.temporaryPath, true);
        }

        private static void DownloadFiles()
        {
            using var client = new HttpClient();
            client.Download($"{Defaults.resourceUrl}/{Defaults.surahsFileName}", surahsFilePath);
            client.Download($"{Defaults.resourceUrl}/{Defaults.translationsFileName}", translationsFilePath);
            client.Download($"{Defaults.resourceUrl}/{Defaults.versesFileName}", versesFilePath);
        }

        private static void ConsumeFiles(Repository repository)
        {
            // Consume the files
            using var surahsReader = new StreamReader(surahsFilePath, Encoding.UTF8);
            using var translationsReader = new StreamReader(translationsFilePath, Encoding.UTF8);
            using var versesReader = new StreamReader(versesFilePath, Encoding.UTF8);
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
                    repository.Create(surah);
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
                repository.Create(ayah);
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
                EnglishName = parts[3],
                TransliterationName = parts[4]
            };
        }

        public static void PopulateAyahFts(this SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO AyahFts(rowid, verse, translation)
                SELECT id, verse, translation FROM Ayah
                WHERE id NOT IN (SELECT rowid FROM AyahFts);
            ";
            command.ExecuteNonQuery();
        }
    }
}