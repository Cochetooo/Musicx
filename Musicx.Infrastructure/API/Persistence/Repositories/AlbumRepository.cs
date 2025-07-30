using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Options;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class AlbumRepository(
    IDbConnectionProvider connection,
    ILoggerProvider loggerProvider) : IAlbumRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM albums WHERE {AlbumColumns.Id} = @id";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM albums WHERE {AlbumColumns.Id} IN (@ids)";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutAlbum?> FindByIdAsync(long id, IQuerySpecification<InAlbum>? albumQuerySpecification = null)
    {
        var sql = new StringBuilder(Select(albumQuerySpecification));
        sql.Append($" WHERE al0.{AlbumColumns.Id} = @id")
            .Append(GroupBy(albumQuerySpecification));
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToAlbum();
    }
    
    public async Task<List<OutAlbum>> FindByArtistIdAsync(long artistId, IQuerySpecification<InAlbum>? albumQuerySpecification = null)
    {
        var sql = Select(albumQuerySpecification);
        sql += $" WHERE al0.{AlbumColumns.ArtistId} = @artistId" + 
               GroupBy(albumQuerySpecification) +
               $" ORDER BY al0.{AlbumColumns.OriginalReleaseDate}, al0.{AlbumColumns.Name}";
        var parameters = new List<NpgsqlParameter>
        {
            new("@artistId", artistId)
        };
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }
    
    public async Task<List<OutAlbum>> FindByGenreIdAsync(long genreId, 
        int genreOptions,
        int skip = 0,
        int take = 100,
        IQuerySpecification<InAlbum>? albumQuerySpecification = null)
    {
        const int validMask = GenreOptions.PrimaryGenre | GenreOptions.InfluenceGenre;

        if ((genreOptions & ~validMask) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(genreOptions),
                $"Invalid genreOptions: {genreOptions}. Must be a combination of GenreOptions.PrimaryGenre (0x1) and/or InfluenceGenre (0x2).");
        }

        var joins = new List<string>();
        var whereConditions = new List<string>();
        var parameters = new List<NpgsqlParameter>
        {
            new("@genreId", genreId),
            new("@skip", skip),
            new("@take", take),
        };

        if ((genreOptions & GenreOptions.PrimaryGenre) != 0)
        {
            joins.Add($" INNER JOIN album_genre pg ON pg.{AlbumGenreColumns.AlbumId} = a.{AlbumColumns.Id}");
            whereConditions.Add($"pg.{AlbumGenreColumns.GenreId} = @genreId");
        }
        
        if ((genreOptions & GenreOptions.InfluenceGenre) != 0)
        {
            joins.Add($" INNER JOIN album_influence ig ON ig.{AlbumInfluenceColumns.AlbumId} = a.{AlbumColumns.Id}");
            whereConditions.Add($"ig.{AlbumInfluenceColumns.GenreId} = @genreId");
        }
        
        var sql = $"""
                  SELECT DISTINCT a.*
                  FROM albums a
                  {string.Join("\n", joins)}
                  WHERE {string.Join(" OR ", whereConditions)}
                  {GroupBy(albumQuerySpecification)}
                  ORDER BY a."ReleaseDate", a."Name"
                  OFFSET @skip LIMIT @take
                  """;
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }

    public async Task<List<OutAlbum>> FindAsync(int skip = 0, int take = 100,
        IQuerySpecification<InAlbum>? albumQuerySpecification = null,
        string? filter = null)
    {
        var sql = Select(albumQuerySpecification);

        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" WHERE similarity(al0.{AlbumColumns.Name}, @filter) > 0.4";
            parameters.Add(new NpgsqlParameter("@filter", filter));
        }
        
        sql += GroupBy(albumQuerySpecification);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" ORDER BY similarity(al0.{AlbumColumns.Name}, @filter) DESC";
        }
        else
        {
            sql += $" ORDER BY al0.{AlbumColumns.Name}";
        }
        
        sql +=  " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }

    public async Task<List<OutAlbum>> FindIn(IEnumerable<long> ids, IQuerySpecification<InAlbum>? albumQuerySpecification = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }
        
        var stringIds = string.Join(",", idList);
        var sql = Select(albumQuerySpecification);
        sql += $" WHERE al0.{AlbumColumns.Id} IN ({stringIds})" +
               GroupBy(albumQuerySpecification) +
               $" ORDER BY al0.{AlbumColumns.OriginalReleaseDate}, al0.{AlbumColumns.Name}";
        
        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }

    public async Task<int> GetCountAsync()
        => await connection.Count("albums");

    public async Task<long> SaveAsync(InAlbum entity)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            if (0 == entity.Id)
            {
                entity.Id = await Insert(entity, conn, transaction);
            }
            else
            {
                
            }
            
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Album : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InAlbum> entities)
    {
        var idList = new List<long>();
        
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            foreach (var entity in entities)
            {
                if (0 == entity.Id)
                {
                    entity.Id = await Insert(entity, conn, transaction);
                }
                else
                {
                
                }
                
                idList.Add(entity.Id);
            }
            
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Album : Could not persist.", ex, _logger);
        }
        
        return idList;
    }

    private static string Select(IQuerySpecification<InAlbum>? querySpecification = null)
    {
        if (querySpecification is not AlbumQuerySpecification albumQuerySpecification)
        {
            return "SELECT al0.* FROM albums al0";
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
            selects.Add($"json_agg(pg.*) FILTER (WHERE pg.{GenreColumns.Id} IS NOT NULL) AS primary_genres");
            joins.Add($"LEFT JOIN album_genre apg ON al0.{AlbumColumns.Id} = apg.{AlbumGenreColumns.AlbumId}");
            joins.Add($"LEFT JOIN genres pg ON apg.{AlbumGenreColumns.GenreId} = pg.{GenreColumns.Id}");
        }

        if (albumQuerySpecification.IncludeInfluenceGenres)
        {
            selects.Add($"json_agg(ig.*) FILTER (WHERE ig.{GenreColumns.Id} IS NOT NULL) AS influence_genres");
            joins.Add($"LEFT JOIN album_influence aig ON al0.{AlbumColumns.Id} = aig.{AlbumInfluenceColumns.AlbumId}");
            joins.Add($"LEFT JOIN genres ig ON aig.{AlbumInfluenceColumns.GenreId} = ig.{GenreColumns.Id}");
        }

        return $"SELECT {string.Join(", ", selects)} FROM albums al0 {string.Join(" ", joins)}";
    }

    private static string GroupBy(IQuerySpecification<InAlbum>? querySpecification = null)
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

    private async Task<long> Insert(InAlbum entity, NpgsqlConnection conn, NpgsqlTransaction transaction)
    {
        long albumId;
        
        var createCommandSql = SqlHelper.Insert("albums",
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
                { AlbumColumns.Language, entity.Language },
                { AlbumColumns.Name, entity.Name },
                { AlbumColumns.OriginalReleaseDate, entity.OriginalReleaseDate },
                { AlbumColumns.ReleaseType, entity.ReleaseType },
                { AlbumColumns.TrackTotal, entity.TrackTotal }
            }, AlbumColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Item1, createCommandSql.Item2));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Item1, conn, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Item2.ToArray());
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
                var primaryGenreSql = SqlHelper.Insert("album_genre",
                    new Dictionary<string, object?>
                    {
                        { AlbumGenreColumns.AlbumId, albumId },
                        { AlbumGenreColumns.GenreId, primaryGenre }
                    });
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(primaryGenreSql.Item1, primaryGenreSql.Item2));

                await using var cmd = new NpgsqlCommand(primaryGenreSql.Item1, conn, transaction);
                cmd.Parameters.AddRange(primaryGenreSql.Item2.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }

        if (null != entity.InfluenceGenreIds)
        {
            foreach (var influenceGenre in entity.InfluenceGenreIds)
            {
                var influenceGenreSql = SqlHelper.Insert("album_influence",
                    new Dictionary<string, object?>
                    {
                        { AlbumInfluenceColumns.AlbumId, albumId },
                        { AlbumInfluenceColumns.GenreId, influenceGenre }
                    });
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(influenceGenreSql.Item1, influenceGenreSql.Item2));

                await using var cmd = new NpgsqlCommand(influenceGenreSql.Item1, conn, transaction);
                cmd.Parameters.AddRange(influenceGenreSql.Item2.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        return albumId;
    }
}