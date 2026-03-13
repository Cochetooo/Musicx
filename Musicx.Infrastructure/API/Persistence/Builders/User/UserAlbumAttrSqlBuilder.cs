using Microsoft.Extensions.Logging;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.Label;
using Musicx.Infrastructure.API.Persistence.Columns.Tag;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Specifications.User;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.User;

internal sealed class UserAlbumAttrSqlBuilder(
    ILoggerProvider loggerProvider) : SqlBuilder<InUserAlbumAttribute>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserAlbumAttrSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InUserAlbumAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = InsertBuilder.Build("user_album_attrs",
            new Dictionary<string, object?>
            {
                { UserAlbumAttrColumns.AlbumId, entity.AlbumId },
                { UserAlbumAttrColumns.UserId, entity.UserId },
                { UserAlbumAttrColumns.CreatedAt, DateTime.Now },
                { UserAlbumAttrColumns.UpdatedAt, DateTime.Now },
                { UserAlbumAttrColumns.CollectionType, entity.CollectionType },
                { UserAlbumAttrColumns.DiscoveryDate, entity.DiscoveryDate },
                { UserAlbumAttrColumns.Rating, entity.Rating },
                { UserAlbumAttrColumns.Review, entity.Review },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));
        
        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }

        return null;
    }

    internal override async Task ExecuteUpdate(InUserAlbumAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = UpdateBuilder.Build("user_album_attrs",
            new Dictionary<string, object?>
            {
                { UserAlbumAttrColumns.UpdatedAt, DateTime.Now },
                { UserAlbumAttrColumns.CollectionType, entity.CollectionType },
                { UserAlbumAttrColumns.DiscoveryDate, entity.DiscoveryDate },
                { UserAlbumAttrColumns.Rating, entity.Rating },
                { UserAlbumAttrColumns.Review, entity.Review },
            },
            new Dictionary<string, object?>
            {
                { UserAlbumAttrColumns.UserId, entity.UserId },
                { UserAlbumAttrColumns.AlbumId, entity.AlbumId },
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));
        
        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
    }

    internal override Task ExecuteUpsert(InUserAlbumAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IJoinSpecification<InUserAlbumAttribute>? spec = null, bool distinct = false)
    {
        if (spec is not UserAlbumAttrJoinSpecification specUserAlbumAttr)
        {
            return distinct
                ? "SELECT DISTINCT uaa0.*, u0.*, al0.* FROM user_album_attrs uaa0 " +
                  $"JOIN users u0 ON uaa0.{UserAlbumAttrColumns.UserId} = u0.{UserColumns.Id} " +
                  $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id} "
                : "SELECT uaa0.*, u0.*, al0.* FROM user_album_attrs uaa0 " +
                  $"JOIN users u0 ON uaa0.{UserAlbumAttrColumns.UserId} = u0.{UserColumns.Id} " +
                  $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id} ";
        }

        var selects = new List<string> { "uaa0.*", "u0.*", "al0.*" };
        var joins = new List<string>
        {
            $"JOIN users u0 ON uaa0.{UserAlbumAttrColumns.UserId} = u0.{UserColumns.Id}",
            $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id}"
        };

        if (specUserAlbumAttr.IncludeAlbumArtists)
        {
            selects.Add("ar0.*");
            joins.Add($"LEFT JOIN artists ar0 ON al0.{AlbumColumns.ArtistId} = ar0.{ArtistColumns.Id}");
        }
        
        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM user_album_attrs uaa0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM user_album_attrs uaa0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IJoinSpecification<InUserAlbumAttribute>? spec = null)
        => string.Empty;

    internal override (string Sql, List<NpgsqlParameter> Parameters) BuildFilteredQuery(
        IFindQuery<InUserAlbumAttribute> query,
        IJoinSpecification<InUserAlbumAttribute>? joinSpec,
        OrderSpecification<InUserAlbumAttribute>? orderSpec,
        PagingOptions? pagingOptions,
        bool countOnly)
    {
        if (query is not UserAlbumAttrFindQuery userAlbumAttrQuery)
        {
            return (string.Empty, []);
        }
        
        var sql = countOnly
            ? "SELECT COUNT(*) FROM user_album_attrs uaa0 "
            : BuildSelect(joinSpec);

        if (countOnly)
        {
            sql += $"JOIN users u0 ON uaa0.{UserAlbumAttrColumns.UserId} = u0.{UserColumns.Id} ";
            sql += $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id} ";
        }
        
        var parameters = new List<NpgsqlParameter>();
        var clauses = new List<string>();
        var joins = new HashSet<string>();
        
        void AddParam(string name, object? value) => parameters.Add(new NpgsqlParameter(name, value ?? DBNull.Value));

        if (userAlbumAttrQuery.UserId is not null)
        {
            clauses.Add($"uaa0.{UserAlbumAttrColumns.UserId} = @userId");
            AddParam("@userId", userAlbumAttrQuery.UserId.Value);
        }
        
        if (userAlbumAttrQuery.AlbumId is not null)
        {
            clauses.Add($"uaa0.{UserAlbumAttrColumns.AlbumId} = @albumId");
            AddParam("@albumId", userAlbumAttrQuery.AlbumId.Value);
        }
        
        if (userAlbumAttrQuery.ArtistId is not null)
        {
            clauses.Add($"al0.{AlbumColumns.ArtistId} = @artistId");
            AddParam("@artistId", userAlbumAttrQuery.ArtistId.Value);
        }
        
        if (userAlbumAttrQuery.ReleaseTypes?.Count > 0)
        {
            clauses.Add($"al0.{AlbumColumns.ReleaseType} = ANY(@releaseTypes)");
            AddParam("@releaseTypes", userAlbumAttrQuery.ReleaseTypes.Select(r => (int)r).ToArray());
        }
        
        if (userAlbumAttrQuery.MinDate is not null)
        {
            clauses.Add($"al0.{AlbumColumns.OriginalReleaseDate} >= @minDate");
            AddParam("@minDate", userAlbumAttrQuery.MinDate.Value);
        }
        
        if (userAlbumAttrQuery.MaxDate is not null)
        {
            clauses.Add($"al0.{AlbumColumns.OriginalReleaseDate} <= @maxDate");
            AddParam("@maxDate", userAlbumAttrQuery.MaxDate.Value);
        }
        
        if (userAlbumAttrQuery.MinRating is not null)
        {
            clauses.Add($"uaa0.{UserAlbumAttrColumns.Rating} >= @minRating");
            AddParam("@minRating", userAlbumAttrQuery.MinRating.Value);
        }
        
        if (userAlbumAttrQuery.MaxRating is not null)
        {
            clauses.Add($"uaa0.{UserAlbumAttrColumns.Rating} <= @maxRating");
            AddParam("@maxRating", userAlbumAttrQuery.MaxRating.Value);
        }
        
        FilterBuilder.AppendTextFilter(userAlbumAttrQuery.Album, $"al0.{AlbumColumns.Name}", "album", clauses, parameters, userAlbumAttrQuery.Search);
        FilterBuilder.AppendTextFilter(userAlbumAttrQuery.Artist, $"ar0.{ArtistColumns.Name}", "artist", clauses, parameters, userAlbumAttrQuery.Search);
        FilterBuilder.AppendTextFilter(userAlbumAttrQuery.User, $"u0.{UserColumns.Name}", "user", clauses, parameters, userAlbumAttrQuery.Search);

        if (userAlbumAttrQuery.RawSearch is not null)
        {
            FilterBuilder.AppendAnyTextFilter(
                userAlbumAttrQuery.RawSearch, 
                [$"al0.{AlbumColumns.Name}", $"ar0.{ArtistColumns.Name}", $"u0.{UserColumns.Name}", $"ar0.{ArtistColumns.CurrentCountry}",
                    $"ar0.{ArtistColumns.OriginCountry}", $"ar0.{ArtistColumns.CurrentRegion}", $"ar0.{ArtistColumns.OriginRegion}",
                    $"ar0.{ArtistColumns.CurrentTown}", $"ar0.{ArtistColumns.OriginTown}", $"al0.{AlbumColumns.Language}"],
                "search",
                clauses,
                parameters,
                userAlbumAttrQuery.Search
            );
        }
        
        if (userAlbumAttrQuery.Country is not null)
        {
            var countryColumns = new[] { $"ar0.{ArtistColumns.CurrentCountry}", $"ar0.{ArtistColumns.OriginCountry}" };
            FilterBuilder.AppendAnyTextFilter(userAlbumAttrQuery.Country, countryColumns, "country", clauses, parameters, userAlbumAttrQuery.Search);
        }

        if (userAlbumAttrQuery.Label is not null)
        {
            joins.Add($"LEFT JOIN releases re0 ON re0.{ReleaseColumns.AlbumId} = al0.{AlbumColumns.Id}");
            joins.Add($"LEFT JOIN labels l0 ON l0.{LabelColumns.Id} = re0.{ReleaseColumns.LabelId}");
            FilterBuilder.AppendTextFilter(userAlbumAttrQuery.Label, $"l0.{LabelColumns.Name}", "label", clauses, parameters, userAlbumAttrQuery.Search);
        }

        if (userAlbumAttrQuery.Tag is not null)
        {
            joins.Add($"LEFT JOIN user_album_tags uat0 ON uat0.{UserAlbumTagColumns.AlbumId} = uaa0.{UserAlbumAttrColumns.AlbumId} AND uat0.{UserAlbumTagColumns.UserId} = uaa0.{UserAlbumAttrColumns.UserId}");
            joins.Add($"LEFT JOIN tags t0 ON t0.{TagColumns.Id} = uat0.{UserAlbumTagColumns.TagId}");
            FilterBuilder.AppendTextFilter(userAlbumAttrQuery.Tag, $"t0.{TagColumns.Name}", "tag", clauses, parameters, userAlbumAttrQuery.Search);
        }

        if (userAlbumAttrQuery.GenreIds?.Count > 0)
        {
            joins.Add($"LEFT JOIN album_genre ag0 ON ag0.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id}");
            clauses.Add($"ag0.{AlbumGenreColumns.GenreId} = ANY(@genreIds)");
            AddParam("@genreIds", userAlbumAttrQuery.GenreIds.ToArray());
        }

        if (userAlbumAttrQuery.InfluenceIds?.Count > 0)
        {
            joins.Add($"LEFT JOIN album_influence ai0 ON ai0.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id}");
            clauses.Add($"ai0.{AlbumInfluenceColumns.GenreId} = ANY(@influenceIds)");
            AddParam("@influenceIds", userAlbumAttrQuery.InfluenceIds.ToArray());
        }

        if (userAlbumAttrQuery.Artist is not null || userAlbumAttrQuery.Country is not null)
        {
            joins.Add($"LEFT JOIN artists ar0 ON al0.{AlbumColumns.ArtistId} = ar0.{ArtistColumns.Id}");
        }

        if (joinSpec is UserAlbumAttrJoinSpecification { IncludeAlbumArtists: true })
        {
            joins.Add($"LEFT JOIN artists ar0 ON al0.{AlbumColumns.ArtistId} = ar0.{ArtistColumns.Id}");
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
            sql += $" ORDER BY uaa0.{UserAlbumAttrColumns.UpdatedAt} DESC, u0.{UserColumns.Name}";
        }
        
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));
        
        return (sql, parameters);
    }
}