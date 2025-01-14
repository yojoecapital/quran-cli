using System.Collections.Generic;
using System.Linq;

namespace QuranCli.Data.Models
{
    public class Grouping : YamlSerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public List<GroupingLink> Links { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("id", Name);
            if (!string.IsNullOrWhiteSpace(Note)) yield return ("note", Note);
            if (Links != null && Links.Count > 0) yield return ("links", Links.Select(link => Repository.Instance.GetDisplayName(link.AyahId1, link.AyahId2)));
        }
    }
}