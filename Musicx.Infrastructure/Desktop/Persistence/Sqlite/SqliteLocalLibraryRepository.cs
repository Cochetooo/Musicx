using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Infrastructure.Desktop.Persistence.Sqlite;

public sealed class SqliteLocalLibraryRepository(SqliteLibraryDatabase database) : ILocalLibraryRepository
{
    public async Task UpsertTracksAsync(IReadOnlyCollection<LocalTrack> tracks, CancellationToken cancellationToken = default)
    {
        if (tracks.Count == 0)
        {
            return;
        }

        await using var connection = database.OpenConnection();
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        foreach (var track in tracks)
        {
            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO local_track(id, file_path, title, artist, album, format, duration_seconds, api_song_id, api_album_id, api_artist_id, is_matched)
                VALUES($id, $filePath, $title, $artist, $album, $format, $durationSeconds, $apiSongId, $apiAlbumId, $apiArtistId, $isMatched)
                ON CONFLICT(file_path) DO UPDATE SET
                    title = excluded.title,
                    artist = excluded.artist,
                    album = excluded.album,
                    format = excluded.format,
                    duration_seconds = excluded.duration_seconds,
                    api_song_id = excluded.api_song_id,
                    api_album_id = excluded.api_album_id,
                    api_artist_id = excluded.api_artist_id,
                    is_matched = excluded.is_matched;
                """;

            command.Parameters.AddWithValue("$id", track.Id.ToString());
            command.Parameters.AddWithValue("$filePath", track.FilePath);
            command.Parameters.AddWithValue("$title", track.Title);
            command.Parameters.AddWithValue("$artist", track.Artist);
            command.Parameters.AddWithValue("$album", track.Album);
            command.Parameters.AddWithValue("$format", track.Format);
            command.Parameters.AddWithValue("$durationSeconds", (int)track.Duration.TotalSeconds);
            command.Parameters.AddWithValue("$apiSongId", (object?)track.ApiSongId ?? DBNull.Value);
            command.Parameters.AddWithValue("$apiAlbumId", (object?)track.ApiAlbumId ?? DBNull.Value);
            command.Parameters.AddWithValue("$apiArtistId", (object?)track.ApiArtistId ?? DBNull.Value);
            command.Parameters.AddWithValue("$isMatched", track.IsMatchedWithApi ? 1 : 0);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocalTrack>> GetTracksAsync(CancellationToken cancellationToken = default)
    {
        var tracks = new List<LocalTrack>();
        await using var connection = database.OpenConnection();
        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, file_path, title, artist, album, format, duration_seconds, api_song_id, api_album_id, api_artist_id, is_matched FROM local_track ORDER BY artist, album, title";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            tracks.Add(new LocalTrack {
                Id = Guid.Parse(reader.GetString(0)),
                FilePath = reader.GetString(1),
                Title = reader.GetString(2),
                Artist = reader.GetString(3),
                Album = reader.GetString(4),
                Format = reader.GetString(5),
                Duration = TimeSpan.FromSeconds(reader.GetInt32(6)),
                ApiSongId = reader.IsDBNull(7) ? null : reader.GetInt64(7),
                ApiAlbumId = reader.IsDBNull(8) ? null : reader.GetInt64(8),
                ApiArtistId = reader.IsDBNull(9) ? null : reader.GetInt64(9),
                IsMatchedWithApi = reader.GetInt32(10) == 1
            });
        }

        return tracks;
    }

    public async Task<IReadOnlyList<string>> GetArtistsAsync(CancellationToken cancellationToken = default)
    {
        var artists = new List<string>();
        await using var connection = database.OpenConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT DISTINCT artist FROM local_track ORDER BY artist";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            artists.Add(reader.GetString(0));
        }

        return artists;
    }
}