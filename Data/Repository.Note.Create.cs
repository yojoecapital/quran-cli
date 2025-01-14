using QuranCli.Data.Models;

namespace QuranCli.Data
{
    internal partial class Repository
    {
        public void CreateOrEdit(AyatNote ayatNote)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO AyatNote (AyahId1, AyahId2, Note)
                VALUES (@AyahId1, @AyahId2, @Note)
            ";
            command.Parameters.AddWithValue("@AyahId1", ayatNote.AyahId1);
            command.Parameters.AddWithValue("@AyahId2", ayatNote.AyahId2);
            command.Parameters.AddWithValue("@Note", ayatNote.Note);
            command.ExecuteNonQuery();
        }

        public void CreateOrEdit(DirectLink directLink)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO DirectLink (AyahId1, AyahId2, AyahId3, AyahId4, Note)
                VALUES (@AyahId1, @AyahId2, @AyahId3, @AyahId4, @Note)
            ";
            command.Parameters.AddWithValue("@AyahId1", directLink.AyahId1);
            command.Parameters.AddWithValue("@AyahId2", directLink.AyahId2);
            command.Parameters.AddWithValue("@AyahId3", directLink.AyahId3);
            command.Parameters.AddWithValue("@AyahId4", directLink.AyahId4);
            command.Parameters.AddWithValue("@Note", directLink.Note);
            command.ExecuteNonQuery();
        }

        public void CreateOrEdit(Grouping grouping)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO Grouping (Name, Note)
                VALUES (@Name, @Note)
            ";
            command.Parameters.AddWithValue("@Name", grouping.Note);
            command.Parameters.AddWithValue("@Note", grouping.Note);
            command.ExecuteNonQuery();
        }

        public void Edit(Grouping grouping)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Grouping
                SET Note = @Note
                WHERE Name = @Name
            ";
            command.Parameters.AddWithValue("@Note", grouping.Note);
            command.Parameters.AddWithValue("@Name", grouping.Name);
            command.ExecuteNonQuery();
        }

        public void Edit(Grouping grouping, string currentName)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Grouping
                SET Note = @Note, Name = @Name
                WHERE Name = @currentName
            ";
            command.Parameters.AddWithValue("@Note", grouping.Note);
            command.Parameters.AddWithValue("@Name", grouping.Name);
            command.Parameters.AddWithValue("@currentName", currentName);
            command.ExecuteNonQuery();
        }
    }
}