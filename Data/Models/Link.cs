namespace QuranCli.Data.Models
{
    public class Link
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int AyahId1 { get; set; }
        public int AyahId2 { get; set; }
        public Group Group { get; set; }
    }

    public class NestedLink
    {
        public int NestedLinkId { get; set; }
        public int NestedLinkGroupId { get; set; }
        public int NestedLinkAyahId1 { get; set; }
        public int NestedLinkAyahId2 { get; set; }
    }
}