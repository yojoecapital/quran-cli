using System.Collections.Generic;
using System.Text;

namespace QuranCli.Data.Models
{
    public class SurahNote : YamlSerializable
    {
        public int Id { get; set; }
        public int SurahId { get; set; }
        public string Note { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("id", Id);
            yield return ("note", Note);
        }
    }
}