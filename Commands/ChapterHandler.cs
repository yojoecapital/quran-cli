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
                            Logger.Message(surah.Id);
                            break;
                        case SurahField.Name:
                            Logger.Message(surah.Name);
                            break;
                        case SurahField.Transliteration:
                            Logger.Message(surah.TransliterationName);
                            break;
                        case SurahField.Translation:
                            Logger.Message(surah.EnglishName);
                            break;
                        case SurahField.Count:
                            Logger.Message(surah.AyahCount);
                            break;
                    }
                    continue;
                }
            }
            else Logger.Message(YamlSerializable.ToYaml(surahs));
            Repository.DisposeOfInstance();
        }
    }
}