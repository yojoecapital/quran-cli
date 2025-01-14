using System.Collections.Generic;
using System.Text;

namespace QuranCli.Data.Models
{
    public class Surah : YamlSerializable
    {
        public int Id { get; set; }
        public int AyahCount { get; set; }
        public int StartAyahId { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string TransliterationName { get; set; }
        public int EndAyahId
        {
            get => StartAyahId + AyahCount - 1;
        }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("number", Id);
            yield return ("count", AyahCount);
            yield return ("name", Name);
            yield return ("transliteration", TransliterationName);
            yield return ("translation", EnglishName);
        }
    }
}