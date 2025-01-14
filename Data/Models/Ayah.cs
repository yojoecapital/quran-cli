using System.Collections.Generic;

namespace QuranCli.Data.Models
{
    public class Ayah : YamlSerializable
    {
        public int Id { get; set; }
        public int SurahId { get; set; }
        public int AyahNumber { get; set; }
        public string Verse { get; set; }
        public string Translation { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("number", AyahNumber);
            yield return ("surah", SurahId);
            yield return ("verse", Verse);
            yield return ("translation", Translation);
        }
    }
}