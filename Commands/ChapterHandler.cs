using System;
using QuranCli.Arguments;
using QuranCli.Data;

namespace QuranCli.Commands
{
    internal static class ChapterHandler
    {
        public static void Handle(SurahSelection selection)
        {
            foreach (var surah in selection.GetSurahs(Repository.Instance))
            {
                Console.WriteLine($"- id: {surah.Id}");
                Console.WriteLine($"  name: {surah.Name}");
                Console.WriteLine($"  transliterationName: {surah.TransliterationName}");
                Console.WriteLine($"  englishName: {surah.EnglishName}");
                Console.WriteLine($"  ayahCount: {surah.AyahCount}");
            }
        }
    }
}