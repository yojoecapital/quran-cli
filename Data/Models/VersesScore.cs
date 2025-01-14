using System.Collections.Generic;

namespace QuranCli.Data.Models
{
    public class VersesScore : YamlSerializable
    {
        public string Verse1 { get; set; }
        public string Verse2 { get; set; }
        public double Score { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("verse1", Verse1);
            yield return ("verse2", Verse2);
            yield return ("score", Score);
        }
    }
}