using System.Collections.Generic;

namespace QuranCli.Data.Yaml
{
    public interface IYamlSerializable
    {
        public IEnumerable<YamlProperty> GetYamlProperties();
        public IEnumerable<YamlProperty> GetYamlProperties(HashSet<string> include)
        {
            foreach (var property in GetYamlProperties())
            {
                if (include.Contains(property.name)) yield return property;
            }
        }
    }
}
