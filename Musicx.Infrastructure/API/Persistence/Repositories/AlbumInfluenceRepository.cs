using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.Shared.Exceptions;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class AlbumInfluenceRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InAlbumInfluence> builder,
    ILoggerProvider loggerProvider) : IAlbumInfluenceRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumInfluenceRepository));
    
    public Task DeleteAsync(long id)
        => throw new NotImplementedException("DeleteAsync is disabled on this repository.");

    public Task DeleteAllAsync(IEnumerable<long> ids)
        => throw new NotImplementedException("DeleteAllAsync is disabled on this repository.");

    public Task<OutAlbumInfluence?> FindByIdAsync(long id, IQuerySpecification<InAlbumInfluence>? albumInfluenceQuerySpecification = null)
        => throw new NotImplementedException("FindByIdAsync is disabled on this repository.");

    public Task<List<OutAlbumInfluence>> FindAsync(long skip = 0, long take = 100,
        bool? filterExact = null, double? filterSimilitude = 0.4, string? filter = null, string? order = null, 
        IQuerySpecification<InAlbumInfluence>? albumInfluenceQuerySpecification = null)
        => throw new NotImplementedException("FindAsync is disabled on this repository.");

    public Task<List<OutAlbumInfluence>> FindIn(IEnumerable<long> ids, 
        IQuerySpecification<InAlbumInfluence>? albumInfluenceQuerySpecification = null)
        => throw new NotImplementedException("FindIn is disabled on this repository.");
    
    public Task<OutAlbumInfluence?> FindOneAsync(long albumId, long genreId, long taggerId)
    {
        throw new NotImplementedException();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("album_genre");

    public async Task<long> SaveAsync(InAlbumInfluence entity)
    {
        await using var conn = connection.CreateConnection();
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

    public async Task<List<long>> SaveAllAsync(IEnumerable<InAlbumInfluence> entities)
    {
        await using var conn = connection.CreateConnection();
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
            throw new RepositoryException("❌ SAVE ALL Album : Could not persist.", ex, _logger);
        }
        
        return [];
    }
}