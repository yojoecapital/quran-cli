using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using QuranCli.Data.Models;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public void Create(Grouping grouping)
        {
            const string sql = @"
                INSERT INTO Grouping (Name, Note)
                VALUES (@Name, @Note);
            ";
            connection.Execute(sql, grouping);
        }

        public void Edit(Grouping grouping)
        {
            const string sql = @"
                UPDATE Grouping
                SET Note = @Note
                WHERE Name = @Name;
            ";
            connection.Execute(sql, grouping);
        }

        public void PopulateGrouping(Grouping grouping)
        {
            grouping.Links = GetGroupingLinksByGroupId(grouping.Id).ToList();
        }

        public Grouping GetGroupingById(int id)
        {
            const string query = @"
                SELECT * FROM Grouping 
                WHERE Id = @id
            ";
            return connection.QueryFirstOrDefault<Grouping>(query, new { id });
        }

        public Grouping GetGroupingByName(string name)
        {
            const string query = @"
                SELECT * FROM Grouping 
                WHERE Name = @name
            ";
            return connection.QueryFirstOrDefault<Grouping>(query, new { name });
        }

        public IEnumerable<Grouping> GetGroupings()
        {
            const string query = @"
                SELECT * FROM Grouping 
            ";
            return connection.Query<Grouping>(query);
        }

        public void DeleteGrouping(string name)
        {
            var group = GetGroupingByName(name);
            if (group == null) return;
            var links = GetGroupingLinksByGroupId(group.Id);
            foreach (var link in links) DeleteGroupingLink(link.Id);
            const string sql = @"
                DELETE FROM Grouping
                WHERE Name = @name;
            ";
            connection.Execute(sql, new { name });
        }
    }
}