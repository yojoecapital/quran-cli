using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Data.Sqlite;
using QuranCli.Data.Yaml;
using QuranCli.Utilities;

namespace QuranCli.Data.Models
{
    public class Reference : IModel
    {
        public int Id { get; set; }
        public int VerseId1 { get; set; }
        public int VerseId2 { get; set; }
        public int NoteId { get; set; }

        private static readonly string propertiesString = $"{nameof(Id)}, {nameof(VerseId1)}, {nameof(VerseId2)}, {nameof(NoteId)}";

        private static Reference PopulateFrom(SqliteDataReader reader)
        {
            return new Reference()
            {
                Id = reader.GetInt32(0),
                VerseId1 = reader.GetInt32(1),
                VerseId2 = reader.GetInt32(2),
                NoteId = reader.GetInt32(3)
            };
        }

        public static void CreateTable()
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                CREATE TABLE IF NOT EXISTS {nameof(Reference)} (
                    {nameof(Id)} INTEGER PRIMARY KEY,
                    {nameof(NoteId)} INTEGER NOT NULL,
                    {nameof(VerseId1)} INTEGER NOT NULL,
                    {nameof(VerseId2)} INTEGER NOT NULL,
                    CHECK ({nameof(VerseId1)} <= {nameof(VerseId2)}),
                    UNIQUE ({nameof(VerseId1)}, {nameof(VerseId2)}),
                    FOREIGN KEY({nameof(NoteId)}) REFERENCES {nameof(Note)}({nameof(Note.Id)})
                );
                CREATE INDEX idx_{nameof(Reference)}_{nameof(NoteId)} ON {nameof(Reference)}({nameof(NoteId)});
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
                INSERT OR IGNORE INTO {nameof(Reference)} ({propertiesString}) 
                VALUES (@{nameof(Id)}, @{nameof(VerseId1)}, @{nameof(VerseId2)}, @{nameof(NoteId)});
            ";
            command.Parameters.AddWithValue($"@{nameof(Id)}", Id);
            command.Parameters.AddWithValue($"@{nameof(VerseId1)}", VerseId1);
            command.Parameters.AddWithValue($"@{nameof(VerseId2)}", VerseId2);
            command.Parameters.AddWithValue($"@{nameof(NoteId)}", NoteId);
            command.ExecuteNonQuery();
        }

        public static IEnumerable<Reference> SelectBetween(int verseId, int verseId2)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                SELECT {propertiesString}
                FROM {nameof(Reference)}
                WHERE {nameof(VerseId1)} <= @{nameof(verseId2)} AND {nameof(VerseId2)} >= @{nameof(verseId)};
            ";
            command.Parameters.AddWithValue($"@{nameof(verseId)}", verseId);
            command.Parameters.AddWithValue($"@{nameof(verseId2)}", verseId2);
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return PopulateFrom(reader);
        }

        public static void DeleteByNoteId(int noteId)
        {
            var command = ConnectionManager.Connection.CreateCommand();
            command.CommandText = @$"
                DELETE FROM {nameof(Reference)}
                WHERE {nameof(NoteId)} = @{nameof(noteId)}
            ";
            command.Parameters.AddWithValue($"@{nameof(noteId)}", noteId);
            command.ExecuteNonQuery();
        }

    }
}