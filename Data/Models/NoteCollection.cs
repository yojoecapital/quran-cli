using System.Collections.Generic;
using System.Linq;

namespace QuranCli.Data.Models
{
    internal class NoteCollection : YamlSerializable
    {
        public IEnumerable<Grouping> Groupings { get; set; }
        public IEnumerable<DirectLink> Links { get; set; }
        public IEnumerable<YamlSerializable> Notes { get; set; }

        protected override IEnumerable<(string name, object value)> GetProperties()
        {
            if (Groupings.Any()) yield return ("GROUPINGS", Groupings);
            if (Links.Any()) yield return ("LINKS", Links);
            if (Notes.Any()) yield return ("NOTES", Notes);
        }

        public static IEnumerable<YamlSerializable> Join(IEnumerable<YamlSerializable> notes1, IEnumerable<YamlSerializable> notes2)
        {
            foreach (var note in notes1) yield return note;
            foreach (var note in notes2) yield return note;
        }
    }
}