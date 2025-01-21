namespace QuranCli.Data.Yaml
{
    public class YamlProperty(string name, object value)
    {
        public readonly string name = name.ToLower();
        public readonly object value = value;
    }
}