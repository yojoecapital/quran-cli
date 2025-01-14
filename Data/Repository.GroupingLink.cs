using System.Collections.Generic;
using System.Linq;
using Dapper;
using QuranCli.Data.Models;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public void Create(GroupingLink groupingLink)
        {
            const string sql = @"
                INSERT OR IGNORE INTO GroupingLink (GroupId, AyahId1, AyahId2)
                VALUES (@GroupId, @AyahId1, @AyahId2)
            ";
            connection.Execute(sql, groupingLink);
        }

        public IEnumerable<GroupingLink> GetGroupingLinksBetween(int ayahId1, int ayahId2)
        {
            const string query = @"
                SELECT * FROM GroupingLink 
                WHERE AyahId1 <= @ayahId2 AND AyahId2 >= @ayahId1;
            ";
            return connection.Query<GroupingLink>(query, new { ayahId1, ayahId2 });
        }

        private IEnumerable<GroupingLink> GetGroupingLinksByGroupId(int groupId)
        {
            const string query = @"
                SELECT * FROM GroupingLink 
                WHERE GroupId = @groupId;
            ";
            return connection.Query<GroupingLink>(query, new { groupId });
        }

        public void PopulateGroupingLink(GroupingLink groupingLink)
        {
            groupingLink.Grouping = GetGroupingById(groupingLink.GroupId);
            PopulateGrouping(groupingLink.Grouping);
            var index = groupingLink.Grouping.Links.FindIndex(link => link.Id == groupingLink.Id);
            groupingLink.Grouping.Links.RemoveAt(index);
        }

        public void DeleteGroupingLink(int id)
        {
            const string sql = @"
                DELETE FROM GroupingLink
                WHERE Id = @id;
            ";
            connection.Execute(sql, new { id });
        }
    }
}