using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.User;

internal sealed class UserFavoriteAlbumRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InUserFavoriteAlbum> builder,
    ILoggerProvider loggerProvider) : IUserFavoriteAlbumRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserFavoriteAlbumRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM user_fav_album WHERE {UserFavoriteAlbumColumns.UserId} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }
    
    public async Task DeleteOneAsync(long userId, long albumId)
    {
        const string sql = $"DELETE FROM user_fav_album " +
                           $"WHERE {UserFavoriteAlbumColumns.UserId} = @userId AND {UserFavoriteAlbumColumns.AlbumId} = @albumId";

        var parameters = new List<NpgsqlParameter>
        {
            new("@userId", userId),
            new("@albumId", albumId)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public Task DeleteAllAsync(IEnumerable<long> ids)
        => throw new NotImplementedException();

    public Task<OutUserFavoriteAlbum?> FindOneByIdAsync(long id, IJoinSpecification<InUserFavoriteAlbum>? joinSpecification = null)
        => throw new NotImplementedException();

    public Task<List<OutUserFavoriteAlbum>> FindAllAsync(bool? filterExact = null, double? filterSimilitude = 0.4D, string? filter = null,
        IJoinSpecification<InUserFavoriteAlbum>? joinSpec = null, OrderSpecification<InUserFavoriteAlbum>? orderSpec = null, PagingOptions? pagingOptions = null)
        => throw new NotImplementedException();

    public Task<List<OutUserFavoriteAlbum>> FindInAsync(IEnumerable<long> ids, IJoinSpecification<InUserFavoriteAlbum>? joinSpec = null, OrderSpecification<InUserFavoriteAlbum>? orderSpec = null)
        => throw new NotImplementedException();
    
    public async Task<IReadOnlyList<OutUserFavoriteAlbum>> FindByUserAsync(long userId)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE {UserFavoriteAlbumColumns.UserId} = @userId ");

        var parameters = new List<NpgsqlParameter>()
        {
            new("@userId", userId),
        };

        sql.Append($" ORDER BY ufal0.{UserFavoriteAlbumColumns.Order}");
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .Select(x => x.FromDicoToUserFavouriteAlbum())
            .ToList();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("user_fav_album");

    public async Task<long> SaveAsync(InUserFavoriteAlbum entity)
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
            throw new RepositoryException("❌ SAVE User Fav Album : Could not persist.", ex, _logger);
        }
        
        return -1;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InUserFavoriteAlbum> entities)
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
            throw new RepositoryException("❌ SAVE ALL User Fav Album : Could not persist.", ex, _logger);
        }
        
        return [];
    }
}