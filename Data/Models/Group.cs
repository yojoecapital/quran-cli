using System.Collections.Generic;

namespace QuranCli.Data.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<NestedLink> Links { get; set; }

    }
}