using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Genre;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Genre;

internal sealed class GenreRelationRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InGenreRelation> builder,
    ILoggerProvider loggerProvider) : IGenreRelationRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreRelationRepository));
    
    public Task DeleteAsync(long id)
        => throw new NotImplementedException("DeleteAsync is disabled on this repository.");

    public Task DeleteAllAsync(IEnumerable<long> ids)
        => throw new NotImplementedException("DeleteAllAsync is disabled on this repository.");

    public Task<OutGenreRelation?> FindByIdAsync(long id, IJoinSpecification<InGenreRelation>? GenreRelationQuerySpecification = null)
        => throw new NotImplementedException("FindByIdAsync is disabled on this repository.");

    public Task<List<OutGenreRelation>> FindAsync(
            bool? filterExact = null, double? filterSimilitude = 0.4, string? filter = null,
            IJoinSpecification<InGenreRelation>? joinSpec = null,
            OrderSpecification<InGenreRelation>? orderSpec = null,
            PagingOptions? pagingOptions = null)
        => throw new NotImplementedException("FindAsync is disabled on this repository.");

    public Task<List<OutGenreRelation>> FindIn(IEnumerable<long> ids, 
            IJoinSpecification<InGenreRelation>? joinSpec = null,
            OrderSpecification<InGenreRelation>? orderSpec = null)
        => throw new NotImplementedException("FindIn is disabled on this repository.");

    public async Task<long> GetCountAsync()
        => await connection.Count("genre_relation");

    public async Task<long> SaveAsync(InGenreRelation entity)
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
            throw new RepositoryException("❌ SAVE Genre Relation : Could not persist.", ex, _logger);
        }
        
        return -1;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InGenreRelation> entities)
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
            throw new RepositoryException("❌ SAVE ALL Genre Relation : Could not persist.", ex, _logger);
        }
        
        return [];
    }
}