using System.Collections.Generic;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Utilities
{
    public class MatchedVerse : IYamlSerializable
    {
        public Verse Verse { get; set; }
        public int Score { get; set; }

        public IEnumerable<YamlProperty> GetYamlProperties()
        {
            yield return new(nameof(Verse), Verse);
            yield return new(nameof(Score), Score);
        }
    }
}