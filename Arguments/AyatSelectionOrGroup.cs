using System.CommandLine.Parsing;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Arguments
{
    internal class AyatSelectionOrGroup
    {
        public enum Type : byte
        {
            Group,
            Selection
        }

        public readonly Type type;
        public readonly Group group;
        public readonly AyatSelection selection;

        private AyatSelectionOrGroup(Group group)
        {
            this.group = group;
            type = Type.Group;
        }
        private AyatSelectionOrGroup(AyatSelection selection)
        {
            this.selection = selection;
            type = Type.Selection;
        }

        public static AyatSelectionOrGroup ArgumentParse(ArgumentResult result)
        {
            if (result.Tokens.Count == 0)
            {
                result.ErrorMessage = $"Neither a selection or group was not provided. Provide one or the other.";
                return null;
            }
            if (result.Tokens.Count > 0)
            {
                var value = result.Tokens[0].ToString();
                if (Repository.Instance.TryGetGroupByName(value, out var group))
                {
                    result.OnlyTake(1);
                    return new AyatSelectionOrGroup(group);
                }
                var selection = AyatSelection.ArgumentParseWithoutIndex(result);
                if (selection == null) return null;
                else return new AyatSelectionOrGroup(selection);
            }
            result.ErrorMessage = $"Could not parse selection or group.";
            return null;
        }
    }
}