using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Commands
{
    internal static class GroupHandler
    {
        public static void Handle(string name, string note)
        {
            if (name == null)
            {
                var groupings = Repository.Instance.GetGroupings();
                Logger.Message(YamlSerializable.ToYaml(groupings));
                return;
            }
            var grouping = Repository.Instance.GetGroupingByName(name);
            if (grouping == null)
            {
                grouping = new Grouping()
                {
                    Name = name,
                    Note = note?.Trim()
                };
                Repository.Instance.Create(grouping);
            }
            else if (note != null)
            {
                grouping = new Grouping()
                {
                    Name = name,
                    Note = note?.Trim()
                };
                Repository.Instance.Edit(grouping);
            }
            grouping = Repository.Instance.GetGroupingByName(name);
            Repository.Instance.PopulateGrouping(grouping);
            Logger.Message(grouping.ToYaml());
        }
    }
}