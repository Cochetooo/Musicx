using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Builders.Core.Commands;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Album;

internal sealed class AlbumInfluenceRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InAlbumInfluence> builder,
    ManyToManySyncService manyToManySyncService,
    ILoggerProvider loggerProvider) : IAlbumInfluenceRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumInfluenceRepository));
    
    public async Task DeleteOneAsync(long albumId, long genreId, long taggerId)
    {
        const string sql = $"DELETE FROM album_influence " +
                           $"WHERE {AlbumInfluenceColumns.TaggerId} = @taggerId AND {AlbumInfluenceColumns.AlbumId} = @albumId AND {AlbumInfluenceColumns.GenreId} = @genreId";

        var parameters = new List<NpgsqlParameter>
        {
            new("@taggerId", taggerId),
            new("@albumId", albumId),
            new("@genreId", genreId)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }
    
    public Task<OutAlbumInfluence?> FindOneAsync(long albumId, long genreId, long taggerId)
        => throw new NotImplementedException("FindOneAsync is disabled on this repository.");

    public async Task<long> CountAsync(IFindQuery<InAlbumInfluence>? query = null,
        IJoinSpecification<InAlbumInfluence>? joinSpec = null)
        => await connection.Count("album_influence");

    public async Task<long> SaveAsync(InAlbumInfluence entity)
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
            throw new RepositoryException("❌ SAVE Album Influence : Could not persist.", ex, _logger);
        }
        
        return -1;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InAlbumInfluence> entities)
    {
        var albumInfluences = entities.ToList();

        if (albumInfluences.Count == 0)
        {
            return [];
        }
        
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            foreach (var group in albumInfluences.GroupBy(x => new { x.AlbumId, x.TaggerId }))
            {
                await manyToManySyncService.SyncAsync(
                    table: "album_influence",
                    keyColumn: AlbumInfluenceColumns.AlbumId,
                    targetColumn: AlbumInfluenceColumns.GenreId,
                    keyValue: group.Key.AlbumId,
                    newValues: group.Select(x => x.GenreId).Distinct().ToList(),
                    connection: conn,
                    transaction: transaction,
                    scopedColumns: new Dictionary<string, object?>
                    {
                        { AlbumInfluenceColumns.TaggerId, group.Key.TaggerId }
                    }
                );
            }

            foreach (var entity in albumInfluences)
            {
                await builder.ExecuteUpsert(entity, conn, transaction);
            }
            
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Album : Could not persist.", ex, _logger);
        }
        
        return [];
    }
}