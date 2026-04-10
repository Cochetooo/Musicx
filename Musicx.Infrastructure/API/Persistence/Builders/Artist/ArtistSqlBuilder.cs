using Microsoft.Extensions.Logging;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Enums;
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
                { ArtistColumns.WikipediaUrl, entity.WikipediaUrl },
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
                { ArtistColumns.WikipediaUrl, entity.WikipediaUrl },
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
        if (spec is not ArtistJoinSpecification typedSpec)
        {
            return $" GROUP BY ar0.{ArtistColumns.Id}";
        }

        var groupings = new List<string>
        {
            $"ar0.{ArtistColumns.Id}"
        };

        if (typedSpec.IncludeStats)
        {
            groupings.Add($"arst0.{ArtistRatingStatColumns.ArtistId}");
            groupings.Add($"arst0.{ArtistRatingStatColumns.Average}");
            groupings.Add($"arst0.{ArtistRatingStatColumns.Count}");
            groupings.Add($"arst0.{ArtistRatingStatColumns.FanAverage}");
            groupings.Add($"arst0.{ArtistRatingStatColumns.FanCount}");
            groupings.Add($"arst0.{ArtistRatingStatColumns.NonFanAverage}");
            groupings.Add($"arst0.{ArtistRatingStatColumns.NonFanCount}");
        }
        
        return $" GROUP BY {string.Join(", ", groupings)}";
    }
    
    internal override (string Sql, List<NpgsqlParameter> Parameters) BuildFilteredQuery(
        IFindQuery<InArtist> query,
        IJoinSpecification<InArtist>? joinSpec,
        OrderSpecification<InArtist>? orderSpec,
        PagingOptions? pagingOptions,
        bool countOnly)
    {
        if (query is not ArtistFindQuery typedQuery)
        {
            return (string.Empty, []);
        }

        var sql = countOnly
            ? "SELECT COUNT(*) FROM artists ar0 "
            : BuildSelect(joinSpec);

        var parameters = new List<NpgsqlParameter>();
        var where = new List<string>();

        if (!string.IsNullOrWhiteSpace(typedQuery.RawSearch?.Value))
        {
            Filter(ref sql, 
                [$"ar0.{ArtistColumns.Name}", 
                    $"ar0.{ArtistColumns.Alias}"], 
                typedQuery.RawSearch.Value ?? string.Empty, 
                parameters, 
                typedQuery.Search?.Exact ?? false, 
                typedQuery.Search?.Similarity ?? 0.4
            );
        }

        if (typedQuery.MinRating is not null)
        {
            where.Add($"COALESCE(arst0.{ArtistRatingStatColumns.Average},0) >= @minRating");
            parameters.Add(new("@minRating", typedQuery.MinRating));
        }

        if (typedQuery.MaxRating is not null)
        {
            where.Add($"COALESCE(arst0.{ArtistRatingStatColumns.Average},0) <= @maxRating"); 
            parameters.Add(new("@maxRating", typedQuery.MaxRating));
        }

        if (typedQuery.MinUserAge is not null)
        {
            where.Add("EXISTS (SELECT 1 FROM user_album_attrs uaa_age JOIN users u_age ON u_age.user_id = uaa_age.user_album_attr_user_id JOIN albums al_age ON al_age.album_id = uaa_age.user_album_attr_album_id WHERE al_age.album_artist_id = ar0.artist_id AND DATE_PART('year', age(now(), u_age.user_birth_date)) >= @minUserAge)"); 
            parameters.Add(new("@minUserAge", typedQuery.MinUserAge));
        }

        if (typedQuery.MaxUserAge is not null)
        {
            where.Add("EXISTS (SELECT 1 FROM user_album_attrs uaa_age2 JOIN users u_age2 ON u_age2.user_id = uaa_age2.user_album_attr_user_id JOIN albums al_age2 ON al_age2.album_id = uaa_age2.user_album_attr_album_id WHERE al_age2.album_artist_id = ar0.artist_id AND DATE_PART('year', age(now(), u_age2.user_birth_date)) <= @maxUserAge)"); 
            parameters.Add(new("@maxUserAge", typedQuery.MaxUserAge));
        }

        if (typedQuery.Discriminator is not null)
        {
            where.Add($"ar0.{ArtistColumns.Discriminator} = @disc"); 
            parameters.Add(new("@disc", typedQuery.Discriminator.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(typedQuery.Country))
        {
            where.Add($"(ar0.{ArtistColumns.OriginCountry} ILIKE @country OR ar0.{ArtistColumns.CurrentCountry} ILIKE @country)"); 
            parameters.Add(new("@country", typedQuery.Country));
        }
        
        if (typedQuery.CreatedAtFrom is not null)
        {
            where.Add($"ar0.{ArtistColumns.CreatedAt} >= @createdAtFrom");
            parameters.Add(new("@createdAtFrom", typedQuery.CreatedAtFrom));
        }

        if (typedQuery.CreatedAtTo is not null)
        {
            where.Add($"ar0.{ArtistColumns.CreatedAt} <= @createdAtTo");
            parameters.Add(new("@createdAtTo", typedQuery.CreatedAtTo));
        }

        if (typedQuery.IsVisible is not null)
        {
            where.Add($"ar0.{ArtistColumns.IsVisible} = @isVisible");
            parameters.Add(new("@isVisible", typedQuery.IsVisible));
        }

        if (typedQuery.MainGenreId is not null)
        {
            where.Add("EXISTS (SELECT 1 FROM jsonb_array_elements(COALESCE(ar0.artist_calculated_genre_counts,'[]')::jsonb) ge JOIN genre_closure gc ON gc.genre_closure_descendant_id = (ge->>'genreId')::bigint WHERE gc.genre_closure_ancestor_id = @mainGenreId)"); 
            parameters.Add(new("@mainGenreId", typedQuery.MainGenreId));
        }
        
        if (typedQuery.PrimaryGenreIds is { Length: > 0 })
        {
            where.Add("EXISTS (SELECT 1 FROM jsonb_array_elements(COALESCE(ar0.artist_calculated_genre_counts,'[]')::jsonb) sge WHERE (sge->>'genreId')::bigint = ANY(@primaryGenreIds))"); 
            parameters.Add(new("@influenceGenreIds", typedQuery.InfluenceGenreIds));
        }

        if (typedQuery.InfluenceGenreIds is { Length: > 0 })
        {
            where.Add("EXISTS (SELECT 1 FROM jsonb_array_elements(COALESCE(ar0.artist_calculated_influence_counts,'[]')::jsonb) ie WHERE (ie->>'genreId')::bigint = ANY(@influenceGenreIds))"); 
            parameters.Add(new("@influenceGenreIds", typedQuery.InfluenceGenreIds));
        }
        
        if (where.Count > 0)
        {
            sql += (sql.Contains(" WHERE ") ? " AND " : " WHERE ") + string.Join(" AND ", where);
        }
        
        //sql += BuildGroupBy(joinSpec);

        if (countOnly)
        {
            return (sql, parameters);
        }

        if (typedQuery.ChartType is not null)
        {
            sql += typedQuery.ChartType switch
            {
                ChartType.Bottom => $" ORDER BY COALESCE(arst0.{ArtistRatingStatColumns.Average},0) ASC, COALESCE(arst0.{ArtistRatingStatColumns.Count},0) DESC",
                ChartType.Popular => $" ORDER BY COALESCE(arst0.{ArtistRatingStatColumns.Count},0) DESC, COALESCE(arst0.{ArtistRatingStatColumns.Average},0) DESC",
                ChartType.Esoteric => $" ORDER BY (COALESCE(arst0.{ArtistRatingStatColumns.Average},0) - (COALESCE(arst0.{ArtistRatingStatColumns.Count},0) * 0.01 * @popWeight)) DESC",
                _ => $" ORDER BY ((COALESCE(arst0.{ArtistRatingStatColumns.Average},0) * (11-@popWeight)) + (LEAST(COALESCE(arst0.{ArtistRatingStatColumns.Count},0),500) * @popWeight)) DESC"
            };
            
            parameters.Add(new NpgsqlParameter("@popWeight", typedQuery.PopularityWeight ?? 5));
        }
        else
        {
            if (orderSpec is not null)
            {
                sql += BuildOrderBy(orderSpec);
            }
            else
            {
                sql += !string.IsNullOrWhiteSpace(query.RawSearch?.Value)
                    ? $" ORDER BY similarity(ar0.{ArtistColumns.Name}, @filter) DESC"
                    : $" ORDER BY ar0.{ArtistColumns.Name}";
            }
        }
        
        sql += " OFFSET @skip LIMIT @take";
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));

        return (sql, parameters);
    }
}