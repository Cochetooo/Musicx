using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Options;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class AlbumRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InAlbum> builder,
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
        var sql = new StringBuilder(builder.BuildSelect(albumQuerySpecification));
        sql.Append($" WHERE al0.{AlbumColumns.Id} = @id")
            .Append(builder.BuildGroupBy(albumQuerySpecification));
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        try
        {
            return result
                .SingleOrDefault()?
                .FromDicoToAlbum();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogCritical(e.StackTrace);
            throw new ApplicationException();
        }
    }
    
    public async Task<List<OutAlbum>> FindByArtistIdAsync(long artistId, 
        IQuerySpecification<InAlbum>? albumQuerySpecification = null)
    {
        var sql = builder.BuildSelect(albumQuerySpecification);
        sql += $" WHERE al0.{AlbumColumns.ArtistId} = @artistId" + 
               builder.BuildGroupBy(albumQuerySpecification) +
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
        long skip = 0,
        long take = 100,
        string? order = null,
        IQuerySpecification<InAlbum>? albumQuerySpecification = null)
    {
        const int validMask = GenreOptions.PrimaryGenre | GenreOptions.InfluenceGenre;

        if ((genreOptions & ~validMask) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(genreOptions),
                $"Invalid genreOptions: {genreOptions}. Must be a combination of GenreOptions.PrimaryGenre (0x1) and/or InfluenceGenre (0x2).");
        }
        
        /*
         * Assure that primary & influence genres are retrieved because this endpoint must need it.
         */

        if (albumQuerySpecification is AlbumQuerySpecification albumSpec)
        {
            albumSpec.IncludePrimaryGenres = true;
            albumSpec.IncludeInfluenceGenres = true;
        }
        else
        {
            albumQuerySpecification = new AlbumQuerySpecification
            {
                IncludePrimaryGenres = true,
                IncludeInfluenceGenres = true
            };
        }
        
        var sql = builder.BuildSelect(albumQuerySpecification);
        var whereConditions = new List<string>();
        var parameters = new List<NpgsqlParameter>
        {
            new("@genreId", genreId),
            new("@skip", skip),
            new("@take", take),
        };

        if ((genreOptions & GenreOptions.PrimaryGenre) != 0)
        {
            whereConditions.Add($"apg.{AlbumGenreColumns.GenreId} = @genreId");
        }
        
        if ((genreOptions & GenreOptions.InfluenceGenre) != 0)
        {
            whereConditions.Add($"aig.{AlbumInfluenceColumns.GenreId} = @genreId");
        }
        
        sql += $"""
                   WHERE {string.Join(" OR ", whereConditions)}
                  {builder.BuildGroupBy(albumQuerySpecification)}
                  {builder.BuildOrderBy($"al0.{AlbumColumns.OriginalReleaseDate}", $"al0.{AlbumColumns.Name}")}
                  OFFSET @skip LIMIT @take
                  """;
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }

    public async Task<List<OutAlbum>> FindByChart(AlbumChartQuery query)
    {
        var sql = builder.BuildSelect(new AlbumQuerySpecification
        {
            IncludeArtist = true,
            IncludeStats = true,
        });

        var (whereClause, orderClause, parameters) = ((AlbumSqlBuilder)builder).BuildChartQuery(query);

        sql += $"""
                {whereClause}
                {orderClause}
                LIMIT @take OFFSET @skip;
                """;
        
        parameters.Add(new NpgsqlParameter("@take", query.Take));
        parameters.Add(new NpgsqlParameter("@skip", query.Skip));
        
        // @TODO count total
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }

    public async Task<List<OutAlbum>> FindAsync(long skip = 0, long take = 100,
        bool? filterExact = null, double? filterSimilitude = 0.4, string? filter = null, string? order = null, 
        IQuerySpecification<InAlbum>? albumQuerySpecification = null)
    {
        var sql = builder.BuildSelect(albumQuerySpecification);

        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.Filter(
                sql: ref sql, 
                column: $"al0.{AlbumColumns.Name}", 
                filter: filter,
                parameters: parameters, 
                filterExact: filterExact, 
                filterSimilitude: filterSimilitude
            );
        }
        
        sql += builder.BuildGroupBy(albumQuerySpecification);

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

    public async Task<List<OutAlbum>> FindIn(IEnumerable<long> ids, 
        IQuerySpecification<InAlbum>? albumQuerySpecification = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }
        
        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(albumQuerySpecification);
        sql += $" WHERE al0.{AlbumColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(albumQuerySpecification) +
               builder.BuildOrderBy(
                   $"al0.{AlbumColumns.OriginalReleaseDate}",
                   $"al0.{AlbumColumns.Name}");
        
        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("albums");

    public async Task<long> GetCountByGenreIdAsync(long genreId)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();

        var parameters = new List<NpgsqlParameter>()
        {
            new("@genreId", genreId),
        };

        var sql = $"SELECT COUNT(*) FROM albums al0 "
                  + $"JOIN album_genre ag0 ON al0.{AlbumColumns.Id} = ag0.{AlbumGenreColumns.AlbumId} "
                  + $" WHERE ag0.{AlbumGenreColumns.GenreId} = @genreId";

        await using var command = new NpgsqlCommand(sql, conn);
        
        command.Parameters.AddRange(parameters.ToArray());
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));

        try
        {
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Could not execute count by genre command for table album.");
            return -1;
        }
    }

    public async Task<long> SaveAsync(InAlbum entity)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            if (0 == entity.Id)
            {
                var result = await builder.ExecuteInsert(entity, conn, transaction);
                if (result is long l)
                {
                    entity.Id = l;
                }
                else
                {
                    _logger.LogWarning("⚠️ Result from Insert is not long: {result}", result?.ToString());
                }
            }
            else
            {
                await builder.ExecuteUpdate(entity, conn, transaction);
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
                    var result = await builder.ExecuteInsert(entity, conn, transaction);
                    if (result is long l)
                    {
                        entity.Id = l;
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Result from Insert is not long: {result}", result?.ToString());
                    }
                }
                else
                {
                    await builder.ExecuteUpdate(entity, conn, transaction);
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
}