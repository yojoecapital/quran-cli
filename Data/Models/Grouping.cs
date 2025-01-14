using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranCli.Data.Models
{
    public class Grouping : YamlSerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public List<NestedGroupingLink> Links { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("name", Name);
            yield return ("note", Note);
            yield return ("links", Links.Select(link => Repository.Instance.GetDisplayName(link.NestedAyahId1, link.NestedAyahId2)));
        }
    }
}