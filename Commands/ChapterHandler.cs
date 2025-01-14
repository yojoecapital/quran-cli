using System;
using QuranCli.Arguments;
using QuranCli.Data;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    internal static class ChapterHandler
    {
        public static void Handle(string selectionString, SurahField? field)
        {
            if (!SurahSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            Logger.Info(selection.GetLog());
            foreach (var surah in selection.GetSurahs())
            {
                if (field.HasValue)
                {
                    switch (field.Value)
                    {
                        case SurahField.Number:
                            Console.WriteLine(surah.Id);
                            break;
                        case SurahField.Name:
                            Console.WriteLine(surah.Name);
                            break;
                        case SurahField.Transliteration:
                            Console.WriteLine(surah.TransliterationName);
                            break;
                        case SurahField.Translation:
                            Console.WriteLine(surah.EnglishName);
                            break;
                        case SurahField.Count:
                            Console.WriteLine(surah.AyahCount);
                            break;
                    }
                    continue;
                }
                Console.WriteLine($"- id: {surah.Id}");
                Console.WriteLine($"  name: {surah.Name}");
                Console.WriteLine($"  transliterationName: {surah.TransliterationName}");
                Console.WriteLine($"  englishName: {surah.EnglishName}");
                Console.WriteLine($"  ayahCount: {surah.AyahCount}");
            }
            Repository.Instance.Dispose();
        }
    }
}