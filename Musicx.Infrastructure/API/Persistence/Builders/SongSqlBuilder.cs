using Microsoft.Extensions.Logging;
using Musicx.Application.Desktop.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class SongSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InSong>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(SongSqlBuilder));

    internal override async Task<object?> ExecuteInsert(InSong entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        long songId;

        var createCommandSql = BuildInsert("songs",
            new Dictionary<string, object?>
            {
                { SongColumns.CreatedAt, DateTime.Now },
                { SongColumns.UpdatedAt, DateTime.Now },
                { SongColumns.AlbumId, entity.AlbumId },
                { SongColumns.ArtistId, entity.ArtistId },
                { SongColumns.DiscNumber, entity.DiscNumber },
                { SongColumns.Duration, entity.Duration },
                { SongColumns.Lyrics, entity.Lyrics },
                { SongColumns.Title, entity.Title },
                { SongColumns.TrackNumber, entity.TrackNumber },
                { SongColumns.Type, entity.Type }
            }, SongColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            songId = (long)(await cmd.ExecuteScalarAsync() ??
                            throw new NullReferenceException("Could not insert entity."));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + songId);

        if (null != entity.PrimaryGenreIds)
        {
            foreach (var primaryGenre in entity.PrimaryGenreIds)
            {
                var primaryGenreSql = BuildInsert("song_genre",
                    new Dictionary<string, object?>
                    {
                        { SongGenreColumns.SongId, songId },
                        { SongGenreColumns.GenreId, primaryGenre },
                    });
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(primaryGenreSql.Query, primaryGenreSql.Parameters));

                await using var cmd = new NpgsqlCommand(primaryGenreSql.Query, connection, transaction);
                cmd.Parameters.AddRange(primaryGenreSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        if (null != entity.InfluenceGenreIds)
        {
            foreach (var influenceGenre in entity.InfluenceGenreIds)
            {
                var influenceGenreSql = BuildInsert("song_influence",
                    new Dictionary<string, object?>
                    {
                        { SongInfluenceColumns.SongId, songId },
                        { SongInfluenceColumns.GenreId, influenceGenre },
                    });
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(influenceGenreSql.Query, influenceGenreSql.Parameters));

                await using var cmd = new NpgsqlCommand(influenceGenreSql.Query, connection, transaction);
                cmd.Parameters.AddRange(influenceGenreSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }

        return songId;
    }

    internal override async Task ExecuteUpdate(InSong entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("songs",
            SongColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { SongColumns.UpdatedAt, DateTime.Now },
                { SongColumns.AlbumId, entity.AlbumId },
                { SongColumns.ArtistId, entity.ArtistId },
                { SongColumns.DiscNumber, entity.DiscNumber },
                { SongColumns.Duration, entity.Duration },
                { SongColumns.Lyrics, entity.Lyrics },
                { SongColumns.Title, entity.Title },
                { SongColumns.TrackNumber, entity.TrackNumber },
                { SongColumns.Type, entity.Type }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteScalarAsync();
        }
        
        if (null != entity.PrimaryGenreIds)
        {
            foreach (var primaryGenre in entity.PrimaryGenreIds)
            {
                var primaryGenreSql = BuildInsert("song_genre",
                    new Dictionary<string, object?>
                    {
                        { SongGenreColumns.SongId, entity.Id },
                        { SongGenreColumns.GenreId, primaryGenre },
                    },
                    conflictAction: SqlConflictAction.Nothing);
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(primaryGenreSql.Query, primaryGenreSql.Parameters));

                await using var cmd = new NpgsqlCommand(primaryGenreSql.Query, connection, transaction);
                cmd.Parameters.AddRange(primaryGenreSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        if (null != entity.InfluenceGenreIds)
        {
            foreach (var influenceGenre in entity.InfluenceGenreIds)
            {
                var influenceGenreSql = BuildInsert("song_influence",
                    new Dictionary<string, object?>
                    {
                        { SongInfluenceColumns.SongId, entity.Id },
                        { SongInfluenceColumns.GenreId, influenceGenre },
                    },
                    conflictAction: SqlConflictAction.Nothing);
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(influenceGenreSql.Query, influenceGenreSql.Parameters));

                await using var cmd = new NpgsqlCommand(influenceGenreSql.Query, connection, transaction);
                cmd.Parameters.AddRange(influenceGenreSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    internal override string BuildSelect(IQuerySpecification<InSong>? querySpecification = null, bool distinct = false)
    {
        if (querySpecification is not SongQuerySpecification songQuerySpecification)
        {
            return distinct
                ? "SELECT DISTINCT s0.* FROM songs s0"
                : "SELECT s0.* FROM songs s0";
        }

        var selects = new List<string> { "s0.*" };
        var joins = new List<string>();

        if (songQuerySpecification.IncludeArtist)
        {
            selects.Add("ar0.*");
            joins.Add($"INNER JOIN artists ar0 ON s0.{SongColumns.ArtistId} = ar0.{ArtistColumns.Id}");
        }
        
        if (songQuerySpecification.IncludeAlbum)
        {
            selects.Add("al0.*");
            joins.Add($"INNER JOIN albums al0 ON s0.{SongColumns.AlbumId} = al0.{AlbumColumns.Id}");

            if (songQuerySpecification.IncludeAlbumArtist)
            {
                selects.Add("alar0.*");
                joins.Add($"INNER JOIN artists alar0 ON al0.{AlbumColumns.ArtistId} = alar0.{ArtistColumns.Id}");
            }
        }
        
        if (songQuerySpecification.IncludePrimaryGenres)
        {
            selects.Add("(SELECT json_agg(pg.*) FROM song_genre spg " +
                        $"JOIN genres pg ON spg.{SongGenreColumns.GenreId} = pg.{GenreColumns.Id} " +
                        $"WHERE spg.{SongGenreColumns.SongId} = s0.{SongColumns.Id}) AS primary_genres");
        }

        if (songQuerySpecification.IncludeInfluenceGenres)
        {
            selects.Add("(SELECT json_agg(ig.*) FROM song_influence sig " +
                        $"JOIN genres ig ON sig.{SongInfluenceColumns.GenreId} = ig.{GenreColumns.Id} " +
                        $"WHERE sig.{SongInfluenceColumns.SongId} = s0.{SongColumns.Id}) AS influence_genres");
        }
        
        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM songs s0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM songs s0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IQuerySpecification<InSong>? querySpecification = null)
    {
        if (querySpecification is not SongQuerySpecification songQuerySpecification)
        {
            return $" GROUP BY s0.{SongColumns.Id}";
        }

        var groupings = new List<string>
        {
            $"s0.{SongColumns.Id}"
        };

        if (songQuerySpecification.IncludeArtist)
        {
            groupings.Add($"ar0.{ArtistColumns.Id}");
        }
        
        if (songQuerySpecification.IncludeAlbum)
        {
            groupings.Add($"al0.{AlbumColumns.Id}");
        }
        
        return $" GROUP BY {string.Join(", ", groupings)}";
    }
}