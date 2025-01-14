using System.Collections.Generic;

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
            yield return ("id", $"G{Id}");
            yield return ("for", Repository.Instance.GetDisplayName(AyahId1, AyahId2));
            if (Grouping != null) yield return ("group", Grouping);
        }
    }
}