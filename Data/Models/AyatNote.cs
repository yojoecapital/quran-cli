using System.Collections.Generic;

namespace QuranCli.Data.Models
{
    public class AyatNote : YamlSerializable
    {
        public int Id { get; set; }
        public int AyahId1 { get; set; }
        public int AyahId2 { get; set; }
        public string Note { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("id", Id);
            yield return ("for", Repository.Instance.GetDisplayName(AyahId1, AyahId2));
            yield return ("note", Note);
        }
    }
}