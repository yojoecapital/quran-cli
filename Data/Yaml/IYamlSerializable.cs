using System.Collections.Generic;

namespace QuranCli.Data.Yaml
{
    public interface IYamlSerializable
    {
        public IEnumerable<YamlProperty> GetYamlProperties();
    }
}
