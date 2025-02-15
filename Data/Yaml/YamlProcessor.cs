using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuranCli.Utilities;

namespace QuranCli.Data.Yaml
{
    public static class YamlProcessor
    {
        public static void Write(IYamlSerializable model)
        {
            Process(Console.Write, model);
            Console.WriteLine();
        }

        public static void Write(IEnumerable<IYamlSerializable> models)
        {
            Process(Console.Write, models);
            Console.WriteLine();
        }

        public static void Write(IEnumerable<string> strings)
        {
            foreach (var item in strings)
            {
                Console.Write("- ");
                ProcessYamlString(Console.Write, item, string.Empty);
                Console.WriteLine();
            }
        }

        public static void Process(Action<string> process, IEnumerable<IYamlSerializable> models)
        {
            if (!models.Any()) return;
            var builder = new StringBuilder();
            var firstModel = models.First();
            process("- ");
            var indent = "  ";
            ProcessProperties(process, firstModel, indent);
            foreach (var model in models.Skip(1))
            {
                process("\n- ");
                ProcessProperties(process, model, indent);
            }
        }

        public static void Process(Action<string> process, IYamlSerializable model) => ProcessProperties(process, model, string.Empty);

        private static void ProcessProperties(Action<string> process, IYamlSerializable model, string indent)
        {
            var properties = model.GetYamlProperties();
            if (!properties.Any()) return;
            var property = properties.First();
            ProcessProperty(process, property, indent);
            foreach (var subproperty in properties.Skip(1))
            {
                process($"\n{indent}");
                ProcessProperty(process, subproperty, indent);
            }
        }

        private static void ProcessProperty(Action<string> process, YamlProperty property, string indent)
        {
            if (property.value is string s)
            {
                process($"{property.name}: ");
                ProcessYamlString(process, s, indent);
            }
            else if (property.value is IYamlSerializable model)
            {
                var nestedIndent = indent + "  ";
                process($"{property.name}:\n{nestedIndent}");
                ProcessProperties(process, model, nestedIndent);
            }
            else if (property.value is IEnumerable<IYamlSerializable> models)
            {
                var nestedIndent = indent + "  ";
                process($"{property.name}:");
                foreach (var nestedModel in models)
                {
                    process($"\n{indent}- ");
                    ProcessProperties(process, nestedModel, nestedIndent);
                }
            }
            else if (property.value is IEnumerable<string> strings)
            {
                var nestedIndent = indent + "  ";
                process($"{property.name}:");
                foreach (var item in strings)
                {
                    process($"\n{indent}- ");
                    ProcessYamlString(process, item, nestedIndent);
                }
            }
            else if (property.value is IEnumerable<ColoredString> coloredStrings)
            {
                process($"{property.name}: ");
                ProcessColoredString(process, coloredStrings, indent);
            }
            else if (property.value is IEnumerable items)
            {
                process($"{property.name}:");
                foreach (var item in items)
                {
                    process($"\n{indent}- {item}");
                }
            }
            else process($"{property.name}: {property.value}");
        }

        private static void ProcessYamlString(Action<string> process, string value, string indent)
        {
            if (string.IsNullOrEmpty(value)) process("\"\"");
            else if (value.Contains(Environment.NewLine))
            {
                var nestedIndent = $"\n{indent}  ";
                process("|");
                foreach (var line in value.Split(Environment.NewLine))
                {
                    process($"{nestedIndent}{line}");
                }
            }
            else if (string.IsNullOrWhiteSpace(value)) process($"\"{value}\"");
            else if (NeedsEscaping(value)) process(EscapeSingleLinedString(value));
            else process(value);
        }

        private static void ProcessColoredString(Action<string> process, IEnumerable<ColoredString> value, string indent)
        {
            if (!value.Any() || value.All(part => string.IsNullOrEmpty(part.text))) process("\"\"");
            else
            {
                var nestedIndent = $"\n{indent}  ";
                process($"|{nestedIndent}");
                foreach (var part in value)
                {
                    if (part.color.HasValue) Console.ForegroundColor = part.color.Value;
                    process(part.text.Replace(Environment.NewLine, nestedIndent));
                    Console.ResetColor();
                }
            }
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
