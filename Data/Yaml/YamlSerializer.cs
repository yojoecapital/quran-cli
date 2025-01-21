using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranCli.Data.Yaml
{
    public static class YamlSerializer
    {
        public static string Serialze(IYamlSerializable model)
        {
            var builder = new StringBuilder();
            AppendProperties(builder, model, string.Empty);
            return builder.ToString();
        }

        public static string Serialze(IEnumerable<IYamlSerializable> models)
        {
            if (!models.Any()) return string.Empty;
            var builder = new StringBuilder();
            var firstModel = models.First();
            builder.Append("- ");
            var indent = "  ";
            AppendProperties(builder, firstModel, indent);
            foreach (var model in models.Skip(1))
            {
                builder.Append("\n- ");
                AppendProperties(builder, model, indent);
            }
            return builder.ToString();
        }

        private static void AppendProperties(StringBuilder builder, IYamlSerializable model, string indent)
        {
            var properties = model.GetYamlProperties();
            if (!properties.Any()) return;
            var property = properties.First();
            AppendProperty(builder, property, indent);
            foreach (var subproperty in properties.Skip(1))
            {
                builder.Append($"\n{indent}");
                AppendProperty(builder, subproperty, indent);
            }
        }

        private static void AppendProperty(StringBuilder builder, YamlProperty property, string indent)
        {
            if (property.value is string s)
            {
                builder.Append($"{property.name}: {ToYamlString(s, indent)}");
            }
            else if (property.value is IYamlSerializable model)
            {
                var nestedIndent = indent + "  ";
                builder.Append($"{property.name}:\n{nestedIndent}");
                AppendProperties(builder, model, nestedIndent);
            }
            else if (property.value is IEnumerable<IYamlSerializable> models)
            {
                var nestedIndent = indent + "  ";
                builder.Append($"{property.name}:");
                foreach (var nestedModel in models)
                {
                    builder.Append($"\n{indent}- ");
                    AppendProperties(builder, nestedModel, nestedIndent);
                }
            }
            else if (property.value is IEnumerable<string> strings)
            {
                var nestedIndent = indent + "  ";
                builder.Append($"{property.name}:");
                foreach (var item in strings)
                {
                    builder.Append($"\n{indent}- {ToYamlString(item, nestedIndent)}");
                }
            }
            else if (property.value is IEnumerable items)
            {
                builder.Append($"{property.name}:");
                foreach (var item in items)
                {
                    builder.Append($"\n{indent}- {item}");
                }
            }
            else builder.Append($"{property.name}: {property.value}");
        }

        private static string ToYamlString(string value, string indent)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";
            if (string.IsNullOrWhiteSpace(value)) return $"\"{new string(' ', value.Length)}\"";
            if (value.Contains('\n'))
            {
                var builder = new StringBuilder();
                builder.Append('|');
                foreach (var line in value.Split('\n'))
                {
                    builder.Append($"\n{indent}  {line}");
                }
                return builder.ToString();
            }
            if (NeedsEscaping(value)) return EscapeSingleLinedString(value);
            return value;
        }

        private static bool NeedsEscaping(string value)
        {
            foreach (var c in value)
            {
                if (char.IsControl(c) || c == '"' || c == '\\' || c == ':') return true;
            }
            return false;
        }

        private static string EscapeSingleLinedString(string value) => $"'{value.Replace("'", "''")}'";
    }
}
