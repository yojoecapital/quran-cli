using System.Collections.Generic;
using QuranCli.Data.Yaml;

namespace QuranCli.Data.Models
{
    public class Note : IModel, IYamlSerializable
    {
        public int Id { get; set; }
        public string Text { get; set; }

        private static readonly string propertiesString = $"{nameof(Id)}, {nameof(Text)}";

        public static void CreateTable()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                CREATE TABLE IF NOT EXISTS {nameof(Note)} (
                    {nameof(Id)} INTEGER PRIMARY KEY,
                    {nameof(Text)} TEXT NOT NULL
                );
            ";
#if DEBUG
            Logger.Message(command.CommandText);
#endif
            command.ExecuteNonQuery();
        }

        public void Insert()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                INSERT INTO {nameof(Note)} ({propertiesString}) 
                VALUES (@{nameof(Id)}, @{nameof(Text)});
            ";
            command.Parameters.AddWithValue($"@{nameof(Id)}", Id);
            command.Parameters.AddWithValue($"@{nameof(Text)}", Text);
            command.ExecuteNonQuery();
        }

        public IEnumerable<YamlProperty> GetYamlProperties()
        {
            throw new System.NotImplementedException();
        }
    }
}