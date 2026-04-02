using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Specifications.Artist;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Artist;

internal sealed class ArtistSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InArtist>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InArtist entity,
        NpgsqlConnection conn, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = InsertBuilder.Build("artists",
            new Dictionary<string, object?>
            {
                { ArtistColumns.CreatedAt, DateTime.Now },
                { ArtistColumns.UpdatedAt, DateTime.Now },
                { ArtistColumns.Alias, entity.Alias },
                { ArtistColumns.ArtworkUrl, entity.ArtworkUrl },
                { ArtistColumns.CurrentCountry, entity.CurrentCountry },
                { ArtistColumns.CurrentRegion, entity.CurrentRegion },
                { ArtistColumns.CurrentTown, entity.CurrentTown },
                { ArtistColumns.Description, entity.Description },
                { ArtistColumns.IsVisible, entity.IsVisible },
                { ArtistColumns.Name, entity.Name },
                { ArtistColumns.OriginCountry, entity.OriginCountry },
                { ArtistColumns.OriginRegion, entity.OriginRegion },
                { ArtistColumns.OriginTown, entity.OriginTown },
                { ArtistColumns.Discriminator, entity.Discriminator },
                { ArtistColumns.FormationDate, entity.FormationDate },
                { ArtistColumns.SplitDate, entity.SplitDate },
                { ArtistColumns.FirstName, entity.FirstName },
                { ArtistColumns.LastName, entity.LastName },
                { ArtistColumns.BirthDate, entity.BirthDate },
                { ArtistColumns.DeathDate, entity.DeathDate }
            }, ArtistColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(createCommandSql.Query, conn, transaction);
        cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
        var artistId = (long)(await cmd.ExecuteScalarAsync() ??
                              throw new NullReferenceException("Could not insert entity."));

        return artistId;
    }

    internal override async Task ExecuteUpdate(InArtist entity,
        NpgsqlConnection conn, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = UpdateBuilder.Build("artists",
            new Dictionary<string, object?>
            {
                { ArtistColumns.UpdatedAt, DateTime.Now },
                { ArtistColumns.Alias, entity.Alias },
                { ArtistColumns.ArtworkUrl, entity.ArtworkUrl },
                { ArtistColumns.CurrentCountry, entity.CurrentCountry },
                { ArtistColumns.CurrentRegion, entity.CurrentRegion },
                { ArtistColumns.CurrentTown, entity.CurrentTown },
                { ArtistColumns.Description, entity.Description },
                { ArtistColumns.IsVisible, entity.IsVisible },
                { ArtistColumns.Name, entity.Name },
                { ArtistColumns.OriginCountry, entity.OriginCountry },
                { ArtistColumns.OriginRegion, entity.OriginRegion },
                { ArtistColumns.OriginTown, entity.OriginTown },
                { ArtistColumns.Discriminator, entity.Discriminator },
                { ArtistColumns.FormationDate, entity.FormationDate },
                { ArtistColumns.SplitDate, entity.SplitDate },
                { ArtistColumns.FirstName, entity.FirstName },
                { ArtistColumns.LastName, entity.LastName },
                { ArtistColumns.BirthDate, entity.BirthDate },
                { ArtistColumns.DeathDate, entity.DeathDate }
            },
            new Dictionary<string, object?>
            {
                { ArtistColumns.Id, entity.Id }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, conn, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteScalarAsync();
    }

    internal override async Task ExecuteUpsert(InArtist entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        if (0 == entity.Id)
        {
            var result = await ExecuteInsert(entity, connection, transaction);
            if (result is long id)
            {
                entity.Id = id;
            }
            return;
        }

        await ExecuteUpdate(entity, connection, transaction);
    }

    internal override string BuildSelect(IJoinSpecification<InArtist>? spec = null, bool distinct = false)
    {
        var selects = new List<string> { "ar0.*" };
        var joins = new List<string>();

        if (spec is ArtistJoinSpecification { IncludeStats: true })
        {
            selects.Add($"arst0.{ArtistRatingStatColumns.ArtistId}");
            selects.Add($"arst0.{ArtistRatingStatColumns.Average}");
            selects.Add($"arst0.{ArtistRatingStatColumns.Count}");
            selects.Add($"arst0.{ArtistRatingStatColumns.FanAverage}");
            selects.Add($"arst0.{ArtistRatingStatColumns.FanCount}");
            selects.Add($"arst0.{ArtistRatingStatColumns.NonFanAverage}");
            selects.Add($"arst0.{ArtistRatingStatColumns.NonFanCount}");
            joins.Add($"""
                        LEFT JOIN LATERAL (
                        SELECT
                                ar0.{ArtistColumns.Id} AS {ArtistRatingStatColumns.ArtistId},
                                ROUND((SUM(((uaa0.{UserAlbumAttrColumns.Rating}::numeric) * (CASE
                                    WHEN al0.{AlbumColumns.ReleaseType} = 1 THEN 1.0
                                    WHEN al0.{AlbumColumns.ReleaseType} = 7 THEN 0.8
                                    WHEN al0.{AlbumColumns.ReleaseType} = 5 THEN 0.7
                                    WHEN al0.{AlbumColumns.ReleaseType} = 4 THEN 0.6
                                    WHEN al0.{AlbumColumns.ReleaseType} = 9 THEN 0.5
                                    WHEN al0.{AlbumColumns.ReleaseType} = 8 THEN 0.4
                                    WHEN al0.{AlbumColumns.ReleaseType} = 3 THEN 0.3
                                    WHEN al0.{AlbumColumns.ReleaseType} = 6 THEN 0.2
                                    ELSE 0.15
                                END))) FILTER (WHERE uaa0.{UserAlbumAttrColumns.Rating} IS NOT NULL))
                                / NULLIF(SUM((CASE
                                    WHEN al0.{AlbumColumns.ReleaseType} = 1 THEN 1.0
                                    WHEN al0.{AlbumColumns.ReleaseType} = 7 THEN 0.8
                                    WHEN al0.{AlbumColumns.ReleaseType} = 5 THEN 0.7
                                    WHEN al0.{AlbumColumns.ReleaseType} = 4 THEN 0.6
                                    WHEN al0.{AlbumColumns.ReleaseType} = 9 THEN 0.5
                                    WHEN al0.{AlbumColumns.ReleaseType} = 8 THEN 0.4
                                    WHEN al0.{AlbumColumns.ReleaseType} = 3 THEN 0.3
                                    WHEN al0.{AlbumColumns.ReleaseType} = 6 THEN 0.2
                                    ELSE 0.15
                                END)) FILTER (WHERE uaa0.{UserAlbumAttrColumns.Rating} IS NOT NULL), 0), 2) AS {ArtistRatingStatColumns.Average},
                                COUNT(uaa0.{UserAlbumAttrColumns.Rating})::bigint AS {ArtistRatingStatColumns.Count},
                                ROUND((SUM(((uaa0.{UserAlbumAttrColumns.Rating}::numeric) * (CASE
                                    WHEN al0.{AlbumColumns.ReleaseType} = 1 THEN 1.0
                                    WHEN al0.{AlbumColumns.ReleaseType} = 7 THEN 0.8
                                    WHEN al0.{AlbumColumns.ReleaseType} = 5 THEN 0.7
                                    WHEN al0.{AlbumColumns.ReleaseType} = 4 THEN 0.6
                                    WHEN al0.{AlbumColumns.ReleaseType} = 9 THEN 0.5
                                    WHEN al0.{AlbumColumns.ReleaseType} = 8 THEN 0.4
                                    WHEN al0.{AlbumColumns.ReleaseType} = 3 THEN 0.3
                                    WHEN al0.{AlbumColumns.ReleaseType} = 6 THEN 0.2
                                    ELSE 0.15
                                END))) FILTER (WHERE uaa0.{UserAlbumAttrColumns.Rating} IS NOT NULL AND COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = true))
                                / NULLIF(SUM((CASE
                                    WHEN al0.{AlbumColumns.ReleaseType} = 1 THEN 1.0
                                    WHEN al0.{AlbumColumns.ReleaseType} = 7 THEN 0.8
                                    WHEN al0.{AlbumColumns.ReleaseType} = 5 THEN 0.7
                                    WHEN al0.{AlbumColumns.ReleaseType} = 4 THEN 0.6
                                    WHEN al0.{AlbumColumns.ReleaseType} = 9 THEN 0.5
                                    WHEN al0.{AlbumColumns.ReleaseType} = 8 THEN 0.4
                                    WHEN al0.{AlbumColumns.ReleaseType} = 3 THEN 0.3
                                    WHEN al0.{AlbumColumns.ReleaseType} = 6 THEN 0.2
                                    ELSE 0.15
                                END)) FILTER (WHERE uaa0.{UserAlbumAttrColumns.Rating} IS NOT NULL AND COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = true), 0), 2) AS {ArtistRatingStatColumns.FanAverage},
                                (COUNT(uaa0.{UserAlbumAttrColumns.Rating}) FILTER (WHERE COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = true))::bigint AS {ArtistRatingStatColumns.FanCount},
                                ROUND((SUM(((uaa0.{UserAlbumAttrColumns.Rating}::numeric) * (CASE
                                    WHEN al0.{AlbumColumns.ReleaseType} = 1 THEN 1.0
                                    WHEN al0.{AlbumColumns.ReleaseType} = 7 THEN 0.8
                                    WHEN al0.{AlbumColumns.ReleaseType} = 5 THEN 0.7
                                    WHEN al0.{AlbumColumns.ReleaseType} = 4 THEN 0.6
                                    WHEN al0.{AlbumColumns.ReleaseType} = 9 THEN 0.5
                                    WHEN al0.{AlbumColumns.ReleaseType} = 8 THEN 0.4
                                    WHEN al0.{AlbumColumns.ReleaseType} = 3 THEN 0.3
                                    WHEN al0.{AlbumColumns.ReleaseType} = 6 THEN 0.2
                                    ELSE 0.15
                                END))) FILTER (WHERE uaa0.{UserAlbumAttrColumns.Rating} IS NOT NULL AND COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = false))
                                / NULLIF(SUM((CASE
                                    WHEN al0.{AlbumColumns.ReleaseType} = 1 THEN 1.0
                                    WHEN al0.{AlbumColumns.ReleaseType} = 7 THEN 0.8
                                    WHEN al0.{AlbumColumns.ReleaseType} = 5 THEN 0.7
                                    WHEN al0.{AlbumColumns.ReleaseType} = 4 THEN 0.6
                                    WHEN al0.{AlbumColumns.ReleaseType} = 9 THEN 0.5
                                    WHEN al0.{AlbumColumns.ReleaseType} = 8 THEN 0.4
                                    WHEN al0.{AlbumColumns.ReleaseType} = 3 THEN 0.3
                                    WHEN al0.{AlbumColumns.ReleaseType} = 6 THEN 0.2
                                    ELSE 0.15
                                END)) FILTER (WHERE uaa0.{UserAlbumAttrColumns.Rating} IS NOT NULL AND COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = false), 0), 2) AS {ArtistRatingStatColumns.NonFanAverage},
                                (COUNT(uaa0.{UserAlbumAttrColumns.Rating}) FILTER (WHERE COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = false))::bigint AS {ArtistRatingStatColumns.NonFanCount}
                            FROM albums al0
                            LEFT JOIN user_album_attrs uaa0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id}
                                AND uaa0.{UserAlbumAttrColumns.Rating} IS NOT NULL
                            LEFT JOIN user_artist_attrs uar0 ON uar0.{UserArtistAttrColumns.UserId} = uaa0.{UserAlbumAttrColumns.UserId}
                                AND uar0.{UserArtistAttrColumns.ArtistId} = ar0.{ArtistColumns.Id}
                            WHERE al0.{AlbumColumns.ArtistId} = ar0.{ArtistColumns.Id}
                        ) arst0 ON TRUE
                        """
                );
        }

        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM artists ar0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM artists ar0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IJoinSpecification<InArtist>? spec = null)
    {
        throw new NotImplementedException();
    }
}