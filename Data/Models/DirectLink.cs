using System.Collections.Generic;

namespace QuranCli.Data.Models
{
    internal class Pair : YamlSerializable
    {
        public string From { get; set; }
        public string To { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("from", From);
            yield return ("to", To);
        }
    }

    internal class DirectLink : YamlSerializable
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public int AyahId1 { get; set; }
        public int AyahId2 { get; set; }
        public int AyahId3 { get; set; }
        public int AyahId4 { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            yield return ("id", Id);
            var pair = new Pair()
            {
                From = Repository.Instance.GetDisplayName(AyahId1, AyahId2),
                To = Repository.Instance.GetDisplayName(AyahId3, AyahId4)
            };
            yield return ("for", pair);
            yield return ("note", Note);
        }
    }
}