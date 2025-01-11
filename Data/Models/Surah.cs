namespace QuranCli.Data.Models
{
    public class Surah
    {
        public int Id { get; set; }
        public int AyahCount { get; set; }
        public int StartAyahId { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string TransliterationName { get; set; }
    }
}