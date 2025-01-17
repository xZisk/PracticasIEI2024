using System.Globalization;
using System.Text;
using Microsoft.Data.Sqlite;

public static class SQLiteExtensions
{
    public static void AddRemoveAccentsFunction(this SqliteConnection connection)
    {
        connection.CreateFunction<string, string>("remove_accents", (input) =>
        {
            if (input == null) return null;
            return string.Concat(
                input.Normalize(NormalizationForm.FormD)
                     .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            ).Normalize(NormalizationForm.FormC);
        });
    }
}