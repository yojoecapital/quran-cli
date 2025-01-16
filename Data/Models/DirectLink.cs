using System.Collections.Generic;

namespace QuranCli.Data.Models
{
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
            yield return ("id", $"D{Id}");
            if (AyahId1 == AyahId3 && AyahId2 == AyahId4)
            {
                yield return ("for", Repository.Instance.GetDisplayName(AyahId1, AyahId2));
            }
            else
            {
                yield return ("from", Repository.Instance.GetDisplayName(AyahId1, AyahId2));
                yield return ("to", Repository.Instance.GetDisplayName(AyahId3, AyahId4));
            }
            if (!string.IsNullOrWhiteSpace(Note)) yield return ("note", Note);
        }
    }
}