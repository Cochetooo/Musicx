using Microsoft.Extensions.Logging;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Album;

internal sealed class AlbumSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InAlbum>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InAlbum entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long albumId;
        
        var createCommandSql = InsertBuilder.Build("albums",
            new Dictionary<string, object?>
            {
                { AlbumColumns.CreatedAt, DateTime.Now },
                { AlbumColumns.UpdatedAt, DateTime.Now },
                { AlbumColumns.ArtistId, entity.ArtistId },
                { AlbumColumns.ArtistAlias, entity.ArtistAlias },
                { AlbumColumns.ArtworkUrl, entity.ArtworkUrl },
                { AlbumColumns.BeginRecordDate, entity.BeginRecordDate },
                { AlbumColumns.DiscTotal, entity.DiscTotal },
                { AlbumColumns.EndRecordDate, entity.EndRecordDate },
                { AlbumColumns.EnglishName, entity.EnglishName },
                { AlbumColumns.IsExplicitContent, entity.IsExplicitContent },
                { AlbumColumns.IsFarRight, entity.IsFarRight },
                { AlbumColumns.IsGraphicContent, entity.IsGraphicContent },
                { AlbumColumns.IsVisible, entity.IsVisible },
                { AlbumColumns.Language, entity.Language },
                { AlbumColumns.Name, entity.Name },
                { AlbumColumns.OriginalReleaseDate, entity.OriginalReleaseDate },
                { AlbumColumns.ReleaseType, entity.ReleaseType },
                { AlbumColumns.TrackTotal, entity.TrackTotal },
                { AlbumColumns.WikipediaUrl, entity.WikipediaUrl }
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
        
        return albumId;
    }

    internal override async Task ExecuteUpdate(InAlbum entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = UpdateBuilder.Build("albums",
            new Dictionary<string, object?>
            {
                { AlbumColumns.UpdatedAt, DateTime.Now },
                { AlbumColumns.ArtistId, entity.ArtistId },
                { AlbumColumns.ArtistAlias, entity.ArtistAlias },
                { AlbumColumns.ArtworkUrl, entity.ArtworkUrl },
                { AlbumColumns.BeginRecordDate, entity.BeginRecordDate },
                { AlbumColumns.DiscTotal, entity.DiscTotal },
                { AlbumColumns.EndRecordDate, entity.EndRecordDate },
                { AlbumColumns.EnglishName, entity.EnglishName },
                { AlbumColumns.IsExplicitContent, entity.IsExplicitContent },
                { AlbumColumns.IsFarRight, entity.IsFarRight },
                { AlbumColumns.IsGraphicContent, entity.IsGraphicContent },
                { AlbumColumns.IsVisible, entity.IsVisible },
                { AlbumColumns.Language, entity.Language },
                { AlbumColumns.Name, entity.Name },
                { AlbumColumns.OriginalReleaseDate, entity.OriginalReleaseDate },
                { AlbumColumns.ReleaseType, entity.ReleaseType },
                { AlbumColumns.TrackTotal, entity.TrackTotal },
                { AlbumColumns.WikipediaUrl, entity.WikipediaUrl }
            },
            new Dictionary<string, object?>
            {
                { AlbumColumns.Id, entity.Id }
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
        
        if (null != entity.ReleaseIds)
        {
            foreach (var release in entity.ReleaseIds)
            {

            }
        }
    }

    internal override async Task ExecuteUpsert(InAlbum entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
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

    internal override string BuildSelect(IJoinSpecification<InAlbum>? querySpecification = null, bool distinct = false)
    {
        if (querySpecification is not AlbumJoinSpecification albumQuerySpecification)
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
            joins.Add($"LEFT JOIN artists ar0 ON al0.{AlbumColumns.ArtistId} = ar0.{ArtistColumns.Id}");
        }

        if (albumQuerySpecification.IncludePrimaryGenres)
        {
            selects.Add("(SELECT json_agg(jsonb_build_object('relation', apg1.*, 'genre', pg.*)) FROM album_genre apg1 " +
                        $"INNER JOIN genres pg ON apg1.{AlbumGenreColumns.GenreId} = pg.{GenreColumns.Id} " +
                        $"WHERE apg1.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id}) AS primary_genres");
            joins.Add($"LEFT JOIN album_genre apg ON apg.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id}");
        }

        if (albumQuerySpecification.IncludeInfluenceGenres)
        {
            selects.Add("(SELECT json_agg(jsonb_build_object('relation', aig1.*, 'genre', ig.*)) FROM album_influence aig1 " +
                        $"INNER JOIN genres ig ON aig1.{AlbumInfluenceColumns.GenreId} = ig.{GenreColumns.Id} " +
                        $"WHERE aig1.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id}) AS influence_genres");
            joins.Add($"LEFT JOIN album_influence aig ON aig.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id}");
        }

        if (albumQuerySpecification.IncludeStats)
        {
            selects.Add("alst0.*");
            selects.Add($"alfst0.{AlbumRatingStatColumns.FanAverage}");
            selects.Add($"alfst0.{AlbumRatingStatColumns.FanCount}");
            selects.Add($"alfst0.{AlbumRatingStatColumns.NonFanAverage}");
            selects.Add($"alfst0.{AlbumRatingStatColumns.NonFanCount}");
            joins.Add($"LEFT JOIN album_rating_stats alst0 ON al0.{AlbumColumns.Id} = alst0.{AlbumRatingStatColumns.AlbumId}");
            joins.Add($"""
                        LEFT JOIN LATERAL (
                        SELECT
                                AVG(uaa0.{UserAlbumAttrColumns.Rating}) FILTER (WHERE COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = true) AS {AlbumRatingStatColumns.FanAverage},
                                (COUNT(uaa0.{UserAlbumAttrColumns.Rating}) FILTER (WHERE COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = true))::int AS {AlbumRatingStatColumns.FanCount},
                                AVG(uaa0.{UserAlbumAttrColumns.Rating}) FILTER (WHERE COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = false) AS {AlbumRatingStatColumns.NonFanAverage},
                                (COUNT(uaa0.{UserAlbumAttrColumns.Rating}) FILTER (WHERE COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = false))::int AS {AlbumRatingStatColumns.NonFanCount}
                                FROM user_album_attrs uaa0
                                    LEFT JOIN user_artist_attrs uar0 ON uar0.{UserArtistAttrColumns.UserId} = uaa0.{UserAlbumAttrColumns.UserId}
                                AND uar0.{UserArtistAttrColumns.ArtistId} = al0.{AlbumColumns.ArtistId}
                                WHERE uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id}
                                AND uaa0.{UserAlbumAttrColumns.Rating} IS NOT NULL
                        ) alfst0 ON TRUE
                        """);
        }

        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM albums al0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM albums al0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IJoinSpecification<InAlbum>? querySpecification = null)
    {
        if (querySpecification is not AlbumJoinSpecification albumQuerySpecification)
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

        if (albumQuerySpecification.IncludeStats)
        {
            groupings.Add($"alst0.{AlbumRatingStatColumns.AlbumId}");
            groupings.Add($"alfst0.{AlbumRatingStatColumns.FanAverage}");
            groupings.Add($"alfst0.{AlbumRatingStatColumns.FanCount}");
            groupings.Add($"alfst0.{AlbumRatingStatColumns.NonFanAverage}");
            groupings.Add($"alfst0.{AlbumRatingStatColumns.NonFanCount}");
        }
        
        return $" GROUP BY {string.Join(", ", groupings)}";
    }

    internal override (string Sql, List<NpgsqlParameter> Parameters) BuildFilteredQuery(
        IFindQuery<InAlbum> query, 
        IJoinSpecification<InAlbum>? joinSpec,
        OrderSpecification<InAlbum>? orderSpec, 
        PagingOptions? pagingOptions, 
        bool countOnly)
    {
        if (query is not AlbumFindQuery typedQuery)
        {
            return (string.Empty, []);
        }

        var sql = countOnly
            ? "SELECT COUNT(*) FROM albums al0 "
            : BuildSelect(joinSpec);
        
        var parameters = new List<NpgsqlParameter>();
        var clauses = new List<string>();
        var joins = new HashSet<string>();
        
        void AddParam(string name, object? value) => parameters.Add(new NpgsqlParameter(name, value ?? DBNull.Value));

        if (typedQuery.RawSearch is not null)
        {
            FilterBuilder.AppendAnyTextFilter(
                typedQuery.RawSearch, 
                [$"al0.{AlbumColumns.Name}"],
                "search",
                clauses,
                parameters,
                typedQuery.Search
            );
        }
        
        if (typedQuery.CreatedAtFrom is not null)
        {
            clauses.Add($"al0.{AlbumColumns.CreatedAt} >= @createdAtFrom");
            AddParam("@createdAtFrom", typedQuery.CreatedAtFrom);
        }

        if (typedQuery.CreatedAtTo is not null)
        {
            clauses.Add($"al0.{AlbumColumns.CreatedAt} <= @createdAtTo");
            AddParam("@createdAtTo", typedQuery.CreatedAtTo);
        }

        if (typedQuery.IsVisible is not null)
        {
            clauses.Add($"al0.{AlbumColumns.IsVisible} = @isVisible");
            AddParam("@isVisible", typedQuery.IsVisible);
        }

        sql += " ";
        foreach (var join in joins)
        {
            if (!sql.Contains(join, StringComparison.Ordinal))
            {
                sql += join + " ";
            }
        }

        if (clauses.Count > 0)
        {
            sql += " WHERE " + string.Join(" AND ", clauses);
        }

        if (countOnly)
        {
            return (sql, parameters);
        }

        if (orderSpec is not null)
        {
            sql += BuildOrderBy(orderSpec);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(typedQuery.RawSearch?.Value))
            {
                sql += $" ORDER BY similarity(al0.{AlbumColumns.Name}, @filter) DESC";
            }
            else
            {
                sql += $" ORDER BY al0.{AlbumColumns.Name}";
            }
        }
        
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));
        
        return (sql, parameters);
    }

    internal (string WhereClause, string OrderClause, List<NpgsqlParameter> Parameters)
        BuildChartQuery(AlbumChartQuery query)
    {
        var whereConditions = new List<string>();
        var parameters = new List<NpgsqlParameter>();

        // 🔹 Countries inclus/exclus
        
        if (query.IncludedCountries?.Length > 0)
        {
            whereConditions.Add($"ar0.{ArtistColumns.CurrentCountry} = ANY(ARRAY[@includedCountries])");
            parameters.Add(new NpgsqlParameter("@includedCountries", 
                string.Join(",", query.IncludedCountries)));
        }
        
        if (query.ExcludedCountries?.Length > 0)
        {
            whereConditions.Add($"ar0.{ArtistColumns.CurrentCountry} <> ALL(ARRAY[@excludedCountries])");
            parameters.Add(new NpgsqlParameter("@excludedCountries", 
                string.Join(",", query.ExcludedCountries)));
        }
        
        // 🔹 Genres inclus/exclus
        
        if (query.IncludedGenres?.Length > 0)
        {
            whereConditions.Add($"al0.{AlbumColumns.Id} IN (SELECT {AlbumGenreColumns.AlbumId} FROM album_genre WHERE {AlbumGenreColumns.GenreId} = ANY(ARRAY[@includedGenres]))");
            parameters.Add(new NpgsqlParameter("@includedGenres", 
                string.Join(",", query.IncludedGenres)));
        }

        if (query.ExcludedGenres?.Length > 0)
        {
            whereConditions.Add($"al0.{AlbumColumns.Id} NOT IN (SELECT {AlbumGenreColumns.AlbumId} FROM album_genre WHERE {AlbumGenreColumns.GenreId} = ANY(ARRAY[@excludedGenres]))");
            parameters.Add(new NpgsqlParameter("@excludedGenres", 
                string.Join(",", query.ExcludedGenres)));
        }
        
        // 🔹 Date de sortie
        
        if (query.MinDate is not null)
        {
            whereConditions.Add($"al0.{AlbumColumns.OriginalReleaseDate} >= @minDate");
            parameters.Add(new NpgsqlParameter("@minDate", query.MinDate));
        }

        if (query.MaxDate is not null)
        {
            whereConditions.Add($"al0.{AlbumColumns.OriginalReleaseDate} <= @maxDate");
            parameters.Add(new NpgsqlParameter("@maxDate", query.MaxDate));
        }
        
        // 🔹 Rating min/max
        
        if (query.MinRating is not null)
        {
            whereConditions.Add($"alst0.{AlbumRatingStatColumns.Average} >= @minRating");
            parameters.Add(new NpgsqlParameter("@minRating", query.MinRating));
        }

        if (query.MaxRating is not null)
        {
            whereConditions.Add($"alst0.{AlbumRatingStatColumns.Average} <= @maxRating");
            parameters.Add(new NpgsqlParameter("@maxRating", query.MaxRating));
        }
        
        // 🔹 Nb de votes min/max
        
        if (query.MinNbRatings is not null)
        {
            whereConditions.Add($"alst0.{AlbumRatingStatColumns.Count} >= @minNbRatings");
            parameters.Add(new NpgsqlParameter("@minNbRatings", query.MinNbRatings));
        }

        if (query.MaxNbRatings is not null)
        {
            whereConditions.Add($"alst0.{AlbumRatingStatColumns.Count} <= @maxNbRatings");
            parameters.Add(new NpgsqlParameter("@maxNbRatings", query.MaxNbRatings));
        }
        
        // 🔹 Filtrage par âge utilisateur (en années)
        
        if (query.MinAge is not null)
        {
            whereConditions.Add(@$"
            EXISTS (
                SELECT 1 
                FROM user_album_attrs uaa2
                JOIN users u2 ON u2.{UserColumns.Id} = uaa2.{UserAlbumAttrColumns.UserId}
                WHERE uaa2.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id}
                AND DATE_PART('year', AGE(u2.{UserColumns.BirthDate})) >= @minAge
            )");
            parameters.Add(new NpgsqlParameter("@minAge", query.MinAge));
        }

        if (query.MaxAge is not null)
        {
            whereConditions.Add($@"
            EXISTS (
                SELECT 1 
                FROM user_album_attrs uaa
                JOIN users u2 ON u2.{UserColumns.Id} = uaa2.{UserAlbumAttrColumns.UserId}
                WHERE uaa2.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id}
                AND DATE_PART('year', AGE(u2.{UserColumns.BirthDate})) <= @maxAge
            )");
            parameters.Add(new NpgsqlParameter("@maxAge", query.MaxAge));
        }
        
        // 🔹 Nom partiel
        
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            whereConditions.Add($"al0.{AlbumColumns.Name} ILIKE CONCAT('%', @name, '%') OR al0.{AlbumColumns.EnglishName} ILIKE CONCAT('%', @name, '%')");
            parameters.Add(new NpgsqlParameter("@name", query.Name));
        }
        
        // 🔹 Release types
        
        if (query.ReleaseTypes?.Length > 0)
        {
            whereConditions.Add($"al0.{AlbumColumns.ReleaseType} = ANY(@releaseTypes)");
            parameters.Add(new NpgsqlParameter("@releaseTypes", query.ReleaseTypes.Select(rt => (int)rt).ToArray()));
        }
        
        // 🔹 DISTINCT ARTISTS
        
        string distinctClause = query.DistinctArtists
            ? $"DISTINCT ON (al0.{AlbumColumns.ArtistId})"
            : "";
        
        // 🔹 Construction du ORDER BY selon ChartType
        string orderClause = query.ChartType switch
        {
            ChartType.Popular => $"ORDER BY alst0.{AlbumRatingStatColumns.Count} DESC NULLS LAST",
            ChartType.Bottom => $"ORDER BY alst0.{AlbumRatingStatColumns.Average} ASC NULLS LAST",
            ChartType.Esoteric => $"ORDER BY (alst0.{AlbumRatingStatColumns.Average} * POWER(1.0 / (alst0.{AlbumRatingStatColumns.Count} + 1), 0.5)) DESC NULLS LAST",
            ChartType.Top => $@"
            ORDER BY (
            ((alst0.{AlbumRatingStatColumns.Average} / 100.0) * POWER(10 - {query.PopularityWeight}, 1.3)) +
            (LOG(alst0.{AlbumRatingStatColumns.Count} + 1) * POWER({query.PopularityWeight}, 2.2))
            ) DESC NULLS LAST",
            _ => $"ORDER BY alst0.{AlbumRatingStatColumns.Average} DESC NULLS LAST"
        };
        
        var whereClause = whereConditions.Count > 0
            ? " WHERE " + string.Join(" AND ", whereConditions)
            : "";

        return (whereClause, orderClause, parameters);
    }
}