using QuranCli.Arguments;
using QuranCli.Data;
using QuranCli.Data.Models;

namespace QuranCli.Utilities
{
    internal static class LinkCreator
    {
        public static void CreateLink(AyatSelection selection, string text)
        {
            var (ayahId1, ayahId2) = selection.GetAyahIds();
            var group = new Group() { Text = text };
            var groupId = Repository.Instance.Create(group);
            var link = new Link()
            {
                GroupId = groupId,
                AyahId1 = ayahId1,
                AyahId2 = ayahId2
            };
            Repository.Instance.Create(link);
        }

        public static void CreateLink(AyatSelection selection, string text, int groupId)
        {
            // FIXME: what am I doing with text here?
            var (ayahId1, ayahId2) = selection.GetAyahIds();
            var link = new Link()
            {
                GroupId = groupId,
                AyahId1 = ayahId1,
                AyahId2 = ayahId2
            };
            Repository.Instance.Create(link);
        }

        public static void CreateLink(AyatSelection selection1, string text, AyatSelection selection2)
        {
            var range1 = selection1.GetAyahIds();
            var range2 = selection2.GetAyahIds();
            var group = new Group() { Text = text };
            var groupId = Repository.Instance.Create(group);
            var link1 = new Link()
            {
                GroupId = groupId,
                AyahId1 = range1.ayahId1,
                AyahId2 = range1.ayahId2
            };
            Repository.Instance.Create(link1);
            var link2 = new Link()
            {
                GroupId = groupId,
                AyahId1 = range2.ayahId1,
                AyahId2 = range2.ayahId2
            };
            Repository.Instance.Create(link2);
        }
    }
}