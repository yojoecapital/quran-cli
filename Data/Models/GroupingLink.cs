namespace QuranCli.Data.Models
{
    public class GroupingLink
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int AyahId1 { get; set; }
        public int AyahId2 { get; set; }
        public Grouping Grouping { get; set; }
    }

    public class NestedGroupingLink
    {
        public int NestedId { get; set; }
        public int NestedGroupId { get; set; }
        public int NestedAyahId1 { get; set; }
        public int NestedAyahId2 { get; set; }
    }
}