using System;
using QuranCli.Data;

namespace QuranCli.Commands
{
    internal static class RemoveHandler
    {
        public static void Handle(string[] idStrings)
        {
            foreach (var idString in idStrings)
            {
                Repository.Instance.DeleteGrouping(idString);
                if (!int.TryParse(idString[1..], out var id)) continue;
                var code = idString[0];
                switch (char.ToUpper(code))
                {
                    case 'S':
                        Repository.Instance.DeleteSurahNote(id);
                        break;
                    case 'A':
                        Repository.Instance.DeleteAyatNote(id);
                        break;
                    case 'D':
                        Repository.Instance.DeleteDirectLink(id);
                        break;
                    case 'G':
                        Repository.Instance.DeleteGroupingLink(id);
                        break;
                }
            }
            Repository.DisposeOfInstance();
        }
    }
}