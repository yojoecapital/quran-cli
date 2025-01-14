using System.Collections.Generic;
using System.Text;

namespace QuranCli.Data.Models
{
    public class SurahNote : YamlSerializable
    {
        public int Id { get; set; }
        public int SurahId1 { get; set; }
        public int SurahId2 { get; set; }
        public string Note { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("id", $"S{Id}");
            if (SurahId1 == SurahId2) yield return ("for", Repository.Instance.GetSurahById(SurahId1).TransliterationName);
            else if (SurahId1 == 1 && SurahId2 == 114) yield return ("for", "The Noble Quran");
            else yield return
            (
                "for",
                $"{Repository.Instance.GetSurahById(SurahId1).TransliterationName} to {Repository.Instance.GetSurahById(SurahId2).TransliterationName}"
            );
            yield return ("note", Note);
        }
    }
}