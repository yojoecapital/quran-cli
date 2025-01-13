using System.Collections.Generic;

namespace QuranCli.Data.Models
{
    public class Grouping
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public List<NestedGroupingLink> Links { get; set; }

    }
}