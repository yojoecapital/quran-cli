namespace QuranCli.Data.Models
{
    public class Ayah
    {
        public int Id { get; set; }
        public int SurahId { get; set; }
        public int AyahNumber { get; set; }
        public string Verse { get; set; }
        public string Translation { get; set; }
    }
}