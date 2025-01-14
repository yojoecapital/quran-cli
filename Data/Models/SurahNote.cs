using System.Collections.Generic;

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
            yield return ("for", Repository.Instance.GetSurahTransliterationDisplayName(SurahId1, SurahId2));
            yield return ("note", Note);
        }
    }
}