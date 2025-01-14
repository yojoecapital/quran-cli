using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranCli.Data.Models
{
    public abstract class YamlSerializable
    {
        protected abstract IEnumerable<(string name, object value)> GetProperties();

        public string ToYaml()
        {
            var builder = new StringBuilder();
            AppendProperties(builder, string.Empty);
            return builder.ToString();
        }

        public static string ToYaml(IEnumerable<YamlSerializable> models)
        {
            if (!models.Any()) return string.Empty;
            var builder = new StringBuilder();
            var firstModel = models.First();
            builder.Append("- ");
            var indent = "  ";
            firstModel.AppendProperties(builder, indent);
            foreach (var model in models.Skip(1))
            {
                builder.Append("\n- ");
                model.AppendProperties(builder, indent);
            }
            return builder.ToString();
        }

        private void AppendProperties(StringBuilder builder, string indent)
        {
            var properties = GetProperties();
            if (!properties.Any()) return;
            var (name, value) = properties.First();
            AppendProperty(builder, name, value, indent);
            foreach (var property in properties.Skip(1))
            {
                builder.Append($"\n{indent}");
                AppendProperty(builder, property.name, property.value, indent);
            }
        }

        private static void AppendProperty(StringBuilder builder, string name, object value, string indent)
        {
            if (value is string s)
            {
                builder.Append($"{name}: {ToYamlString(s, indent)}");
            }
            else if (value is YamlSerializable model)
            {
                var nestedIndent = indent + "  ";
                builder.Append($"{name}:\n{nestedIndent}");
                model.AppendProperties(builder, nestedIndent);
            }
            else if (value is IEnumerable<YamlSerializable> models)
            {
                var nestedIndent = indent + "  ";
                builder.Append($"{name}:");
                foreach (var nestedModel in models)
                {
                    builder.Append($"\n{indent}- ");
                    nestedModel.AppendProperties(builder, nestedIndent);
                }
            }
            else if (value is IEnumerable<string> strings)
            {
                var nestedIndent = indent + "  ";
                builder.Append($"{name}:");
                foreach (var item in strings)
                {
                    builder.Append($"\n{indent}- {ToYamlString(item, nestedIndent)}");
                }
            }
            else if (value is IEnumerable items)
            {
                builder.Append($"{name}:");
                foreach (var item in items)
                {
                    builder.Append($"\n{indent}- {item}");
                }
            }
            else builder.Append($"{name}: {value}");
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
