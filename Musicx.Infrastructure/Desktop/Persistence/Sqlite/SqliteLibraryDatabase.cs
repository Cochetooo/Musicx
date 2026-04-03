using Microsoft.Data.Sqlite;

namespace Musicx.Infrastructure.Desktop.Persistence.Sqlite;

public sealed class SqliteLibraryDatabase
{
    private readonly string _connectionString;

    public SqliteLibraryDatabase()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Musicx");
        Directory.CreateDirectory(appDataPath);
        var dbPath = Path.Combine(appDataPath, "musicx-desktop.db");
        _connectionString = $"Data Source={dbPath}";

        Initialize();
    }

    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private void Initialize()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS library_profile (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                folder_paths TEXT NOT NULL,
                auto_scan INTEGER NOT NULL,
                auto_hydrate INTEGER NOT NULL,
                is_active INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS local_track (
                id TEXT PRIMARY KEY,
                file_path TEXT NOT NULL UNIQUE,
                title TEXT NOT NULL,
                artist TEXT NOT NULL,
                album TEXT NOT NULL,
                format TEXT NOT NULL,
                duration_seconds INTEGER NOT NULL,
                api_song_id INTEGER NULL,
                api_album_id INTEGER NULL,
                api_artist_id INTEGER NULL,
                is_matched INTEGER NOT NULL
            );
            """;

        command.ExecuteNonQuery();

        using var migrationCommand = connection.CreateCommand();
        migrationCommand.CommandText =
            """
            ALTER TABLE local_track ADD COLUMN api_song_id INTEGER NULL;
            ALTER TABLE local_track ADD COLUMN api_album_id INTEGER NULL;
            ALTER TABLE local_track ADD COLUMN api_artist_id INTEGER NULL;
            """;
        try
        {
            migrationCommand.ExecuteNonQuery();
        }
        catch
        {
            // Column already exists, safe to ignore for local migrations.
        }
    }
}