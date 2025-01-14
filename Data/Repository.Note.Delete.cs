namespace QuranCli.Data
{
    internal partial class Repository
    {
        public void DeleteAyatNotesBetween(int ayahId1, int ayahId2)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM AyatNote
                WHERE AyahId1 <= @ayahId2 AND AyahId2 >= @ayahId1;
            ";
            command.Parameters.AddWithValue("@ayahId1", ayahId1);
            command.Parameters.AddWithValue("@ayahId2", ayahId2);
            command.ExecuteNonQuery();
        }

        public void DeleteAyatNote(int id, int ayahId1, int ayahId2)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM AyatNote
                WHERE Id = @id AND AyahId1 <= @ayahId2 AND AyahId2 >= @ayahId1;
            ";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@ayahId1", ayahId1);
            command.Parameters.AddWithValue("@ayahId2", ayahId2);
            command.ExecuteNonQuery();
        }
    }
}