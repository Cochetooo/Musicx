using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class AlbumSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InAlbum>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InAlbum entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long albumId;
        
        var createCommandSql = BuildInsert("albums",
            new Dictionary<string, object?>
            {
                { AlbumColumns.CreatedAt, DateTime.Now },
                { AlbumColumns.UpdatedAt, DateTime.Now },
                { AlbumColumns.ArtistId, entity.ArtistId },
                { AlbumColumns.ArtworkUrl, entity.ArtworkUrl },
                { AlbumColumns.BeginRecordDate, entity.BeginRecordDate },
                { AlbumColumns.DiscTotal, entity.DiscTotal },
                { AlbumColumns.EndRecordDate, entity.EndRecordDate },
                { AlbumColumns.IsFarRight, entity.IsFarRight },
                { AlbumColumns.IsNsfw, entity.IsNsfw },
                { AlbumColumns.Language, entity.Language },
                { AlbumColumns.Name, entity.Name },
                { AlbumColumns.OriginalReleaseDate, entity.OriginalReleaseDate },
                { AlbumColumns.ReleaseType, entity.ReleaseType },
                { AlbumColumns.TrackTotal, entity.TrackTotal }
            }, AlbumColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            albumId = (long)(await cmd.ExecuteScalarAsync() ??
                             throw new NullReferenceException("Could not insert entity."));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + albumId);

        if (null != entity.ReleaseIds)
        {
            foreach (var release in entity.ReleaseIds)
            {

            }
        }

        if (null != entity.PrimaryGenreIds)
        {
            foreach (var primaryGenre in entity.PrimaryGenreIds)
            {
                var primaryGenreSql = BuildInsert("album_genre",
                    new Dictionary<string, object?>
                    {
                        { AlbumGenreColumns.AlbumId, albumId },
                        { AlbumGenreColumns.GenreId, primaryGenre }
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
                var influenceGenreSql = BuildInsert("album_influence",
                    new Dictionary<string, object?>
                    {
                        { AlbumInfluenceColumns.AlbumId, albumId },
                        { AlbumInfluenceColumns.GenreId, influenceGenre }
                    });
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(influenceGenreSql.Query, influenceGenreSql.Parameters));

                await using var cmd = new NpgsqlCommand(influenceGenreSql.Query, connection, transaction);
                cmd.Parameters.AddRange(influenceGenreSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        return albumId;
    }

    internal override async Task ExecuteUpdate(InAlbum entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("albums",
            AlbumColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { AlbumColumns.UpdatedAt, DateTime.Now },
                { AlbumColumns.ArtistId, entity.ArtistId },
                { AlbumColumns.ArtworkUrl, entity.ArtworkUrl },
                { AlbumColumns.BeginRecordDate, entity.BeginRecordDate },
                { AlbumColumns.DiscTotal, entity.DiscTotal },
                { AlbumColumns.EndRecordDate, entity.EndRecordDate },
                { AlbumColumns.IsFarRight, entity.IsFarRight },
                { AlbumColumns.IsNsfw, entity.IsNsfw },
                { AlbumColumns.Language, entity.Language },
                { AlbumColumns.Name, entity.Name },
                { AlbumColumns.OriginalReleaseDate, entity.OriginalReleaseDate },
                { AlbumColumns.ReleaseType, entity.ReleaseType },
                { AlbumColumns.TrackTotal, entity.TrackTotal }
        });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteScalarAsync();
        }
        
        if (null != entity.ReleaseIds)
        {
            foreach (var release in entity.ReleaseIds)
            {

            }
        }

        if (null != entity.PrimaryGenreIds)
        {
            foreach (var primaryGenre in entity.PrimaryGenreIds)
            {
                var primaryGenreSql = BuildInsert("album_genre",
                    new Dictionary<string, object?>
                    {
                        { AlbumGenreColumns.AlbumId, entity.Id },
                        { AlbumGenreColumns.GenreId, primaryGenre }
                    },
                    conflictAction: SqlConflictAction.Nothing
                );
                
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
                var influenceGenreSql = BuildInsert("album_influence",
                    new Dictionary<string, object?>
                    {
                        { AlbumInfluenceColumns.AlbumId, entity.Id },
                        { AlbumInfluenceColumns.GenreId, influenceGenre }
                    },
                    conflictAction: SqlConflictAction.Nothing
                );
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(influenceGenreSql.Query, influenceGenreSql.Parameters));

                await using var cmd = new NpgsqlCommand(influenceGenreSql.Query, connection, transaction);
                cmd.Parameters.AddRange(influenceGenreSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    internal override string BuildSelect(IQuerySpecification<InAlbum>? querySpecification = null, bool distinct = false)
    {
        if (querySpecification is not AlbumQuerySpecification albumQuerySpecification)
        {
            return distinct 
                ? "SELECT DISTINCT al0.* FROM albums al0" 
                : "SELECT al0.* FROM albums al0";
        }

        var selects = new List<string> { "al0.*" };
        var joins = new List<string>();

        if (albumQuerySpecification.IncludeArtist)
        {
            selects.Add("ar0.*");
            joins.Add($"INNER JOIN artists ar0 ON al0.{AlbumColumns.ArtistId} = ar0.{ArtistColumns.Id}");
        }

        if (albumQuerySpecification.IncludePrimaryGenres)
        {
            selects.Add("(SELECT json_agg(pg.*) FROM album_genre apg1 " +
                        $"INNER JOIN genres pg ON apg1.{AlbumGenreColumns.GenreId} = pg.{GenreColumns.Id} " +
                        $"WHERE apg1.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id}) AS primary_genres");
            joins.Add($"LEFT JOIN album_genre apg ON apg.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id}");
        }

        if (albumQuerySpecification.IncludeInfluenceGenres)
        {
            selects.Add("(SELECT json_agg(ig.*) FROM album_influence aig1 " +
                        $"INNER JOIN genres ig ON aig1.{AlbumInfluenceColumns.GenreId} = ig.{GenreColumns.Id} " +
                        $"WHERE aig1.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id}) AS influence_genres");
            joins.Add($"LEFT JOIN album_influence aig ON aig.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id}");
        }

        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM albums al0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM albums al0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IQuerySpecification<InAlbum>? querySpecification = null)
    {
        if (querySpecification is not AlbumQuerySpecification albumQuerySpecification)
        {
            return $" GROUP BY al0.{AlbumColumns.Id}";
        }

        var groupings = new List<string>
        {
            $"al0.{AlbumColumns.Id}"
        };

        if (albumQuerySpecification.IncludeArtist)
        {
            groupings.Add($"ar0.{ArtistColumns.Id}");
        }
        
        return $" GROUP BY {string.Join(", ", groupings)}";
    }
}