using System;
using QuranCli.Arguments;
using QuranCli.Data;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Commands
{
    internal static class ChapterHandler
    {
        public static void Handle(string selectionString, SurahField? field)
        {
            if (!SurahSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            Logger.Info(selection.GetLog());
            var surahs = selection.GetSurahs();
            if (field.HasValue)
            {
                foreach (var surah in surahs)
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
            }
            else Console.WriteLine(YamlSerializable.ToYaml(surahs));
            Repository.Instance.Dispose();
        }
    }
}