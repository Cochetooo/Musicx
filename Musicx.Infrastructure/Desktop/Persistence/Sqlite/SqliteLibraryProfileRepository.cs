using System.Text.Json;
using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Infrastructure.Desktop.Persistence.Sqlite;

public sealed class SqliteLibraryProfileRepository(SqliteLibraryDatabase database) : ILibraryProfileRepository
{
    public async Task<LibraryProfile?> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = database.OpenConnection();
        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, name, folder_paths, auto_scan, auto_hydrate FROM library_profile WHERE is_active = 1 LIMIT 1";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new LibraryProfile {
            Id = Guid.Parse(reader.GetString(0)),
            Name = reader.GetString(1),
            FolderPaths = JsonSerializer.Deserialize<List<string>>(reader.GetString(2)) ?? [],
            AutoScanOnStartup = reader.GetInt32(3) == 1,
            AutoHydrateMetadata = reader.GetInt32(4) == 1
        };
    }

    public async Task<LibraryProfile> UpsertAsync(LibraryProfile profile, CancellationToken cancellationToken = default)
    {
        var profileId = profile.Id == Guid.Empty ? Guid.NewGuid() : profile.Id;
        await using var connection = database.OpenConnection();

        await using (var disableActive = connection.CreateCommand())
        {
            disableActive.CommandText = "UPDATE library_profile SET is_active = 0";
            await disableActive.ExecuteNonQueryAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO library_profile(id, name, folder_paths, auto_scan, auto_hydrate, is_active)
            VALUES($id, $name, $folderPaths, $autoScan, $autoHydrate, 1)
            ON CONFLICT(id) DO UPDATE SET
                name = excluded.name,
                folder_paths = excluded.folder_paths,
                auto_scan = excluded.auto_scan,
                auto_hydrate = excluded.auto_hydrate,
                is_active = 1;
            """;

        command.Parameters.AddWithValue("$id", profileId.ToString());
        command.Parameters.AddWithValue("$name", profile.Name);
        command.Parameters.AddWithValue("$folderPaths", JsonSerializer.Serialize(profile.FolderPaths));
        command.Parameters.AddWithValue("$autoScan", profile.AutoScanOnStartup ? 1 : 0);
        command.Parameters.AddWithValue("$autoHydrate", profile.AutoHydrateMetadata ? 1 : 0);

        await command.ExecuteNonQueryAsync(cancellationToken);
        profile.Id = profileId;
        
        return profile;
    }
}