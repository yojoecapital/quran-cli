using System.Collections.Generic;
using System.Text;

namespace QuranCli.Data.Models
{
    public class GroupingLink : YamlSerializable
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int AyahId1 { get; set; }
        public int AyahId2 { get; set; }
        public Grouping Grouping { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("id", Id);
            yield return ("for", Repository.Instance.GetDisplayName(AyahId1, AyahId2));
        }
    }

    public class NestedGroupingLink
    {
        public int NestedId { get; set; }
        public int NestedGroupId { get; set; }
        public int NestedAyahId1 { get; set; }
        public int NestedAyahId2 { get; set; }
    }
}