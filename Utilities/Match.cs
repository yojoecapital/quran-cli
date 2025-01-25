using System.Collections.Generic;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Utilities
{
    public class Match<T> : IYamlSerializable where T : IModel
    {
        public T Result { get; set; }
        public int Score { get; set; }

        public IEnumerable<YamlProperty> GetYamlProperties()
        {
            yield return new(nameof(Result), Result);
            yield return new(nameof(Score), Score);
        }
    }
}