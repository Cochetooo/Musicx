using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Album;

internal sealed class AlbumGenreRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InAlbumGenre> builder,
    ILoggerProvider loggerProvider) : IAlbumGenreRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumGenreRepository));
    
    public Task DeleteAsync(long id)
        => throw new NotImplementedException("DeleteAsync is disabled on this repository.");

    public Task DeleteAllAsync(IEnumerable<long> ids)
        => throw new NotImplementedException("DeleteAllAsync is disabled on this repository.");

    public Task<OutAlbumGenre?> FindOneByIdAsync(long id, IJoinSpecification<InAlbumGenre>? albumGenreQuerySpecification = null)
        => throw new NotImplementedException("FindByIdAsync is disabled on this repository.");

    public Task<List<OutAlbumGenre>> FindAllAsync(bool? filterExact = null, double? filterSimilitude = 0.4D,
        string? filter = null,
        IJoinSpecification<InAlbumGenre>? joinSpec = null,
        OrderSpecification<InAlbumGenre>? orderSpec = null,
        PagingOptions? pagingOptions = null)
        => throw new NotImplementedException("FindAsync is disabled on this repository.");

    public Task<List<OutAlbumGenre>> FindInAsync(IEnumerable<long> ids,
        IJoinSpecification<InAlbumGenre>? joinSpec = null,
        OrderSpecification<InAlbumGenre>? orderSpec = null)
        => throw new NotImplementedException("FindIn is disabled on this repository.");

    public async Task<OutAlbumGenre?> FindOneAsync(long albumId, long genreId, long taggerId)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE {AlbumGenreColumns.AlbumId} = @albumId AND {AlbumGenreColumns.GenreId} = @genreId AND {AlbumGenreColumns.TaggerId} = @taggerId");

        var parameters = new List<NpgsqlParameter>
        {
            new("@albumId", albumId),
            new("@genreId", genreId),
            new("@taggerId", taggerId),
        };
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToAlbumGenre();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("album_genre");

    public async Task<long> SaveAsync(InAlbumGenre entity)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            await builder.ExecuteUpsert(entity, conn, transaction);
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Album Genre : Could not persist.", ex, _logger);
        }
        
        return -1;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InAlbumGenre> entities)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            foreach (var entity in entities)
            {
                await builder.ExecuteUpsert(entity, conn, transaction);
            }
            
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Album Genre : Could not persist.", ex, _logger);
        }
        
        return [];
    }
}