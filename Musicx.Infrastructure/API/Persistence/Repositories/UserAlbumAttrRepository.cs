using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class UserAlbumAttrRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InUserAlbumAttribute> builder,
    ILoggerProvider loggerProvider) : IUserAlbumAttrsRepository 
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserAlbumAttrRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM user_album_attrs WHERE {UserAlbumAttrColumns.UserId} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }
    
    public async Task DeleteAsync(long userId, long albumId)
    {
        const string sql = $"DELETE FROM user_album_attrs " +
                           $"WHERE {UserAlbumAttrColumns.UserId} = @userId AND {UserAlbumAttrColumns.AlbumId} = @albumId";

        var parameters = new List<NpgsqlParameter>
        {
            new("@userId", userId),
            new("@albumId", albumId)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public Task DeleteAllAsync(IEnumerable<long> ids)
        => throw new NotImplementedException();

    public Task<OutUserAlbumAttribute?> FindByIdAsync(long id, IQuerySpecification<InUserAlbumAttribute>? songQuerySpecification = null)
        => throw new NotImplementedException();

    public Task<List<OutUserAlbumAttribute>> FindAsync(int skip = 0, int take = 100, IQuerySpecification<InUserAlbumAttribute>? songQuerySpecification = null, string? filter = null)
        => throw new NotImplementedException();

    public Task<List<OutUserAlbumAttribute>> FindIn(IEnumerable<long> ids, IQuerySpecification<InUserAlbumAttribute>? songQuerySpecification = null)
        => throw new NotImplementedException();

    public Task<long> GetCountAsync()
        => throw new NotImplementedException();

    public async Task<long> SaveAsync(InUserAlbumAttribute entity)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            var exist = await FindOneAlbumFromUserAsync(entity.UserId, entity.AlbumId);
            
            if (exist is null)
            {
                await builder.ExecuteInsert(entity, conn, transaction);
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
        
        return -1;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InUserAlbumAttribute> entities)
        => throw new NotImplementedException();

    public async Task<long> CountByAlbumIdAsync(long albumId)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        var sql = $"SELECT COUNT(*) FROM user_album_attrs WHERE {UserAlbumAttrColumns.AlbumId} = @albumId";
        
        await using var command = new NpgsqlCommand(sql, conn);
        var parameters = new List<NpgsqlParameter>()
        {
            new("@albumId", albumId)
        };
        
        command.Parameters.AddRange(parameters.ToArray());
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));

        try
        {
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Could not execute cound by album command for table user_album_attrs.");
            return -1;
        }
    }

    public async Task<long> CountByUserIdAsync(long userId)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        var sql = $"SELECT COUNT(*) FROM user_album_attrs WHERE {UserAlbumAttrColumns.UserId} = @userId";
        
        await using var command = new NpgsqlCommand(sql, conn);
        var parameters = new List<NpgsqlParameter>()
        {
            new("@userId", userId)
        };
        
        command.Parameters.AddRange(parameters.ToArray());
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));

        try
        {
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Could not execute cound by user command for table user_album_attrs.");
            return -1;
        }
    }

    public async Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId, int skip = 0, int take = 100)
    {
        var sql = builder.BuildSelect(null);
        var parameters = new List<NpgsqlParameter>();

        sql += $" WHERE {UserAlbumAttrColumns.AlbumId} = @albumId";
        parameters.Add(new NpgsqlParameter("@albumId", albumId));

        sql += builder.BuildOrderBy($"uaa0.{UserAlbumAttrColumns.UpdatedAt} DESC", $"u0.{UserColumns.Name}");
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToUserAlbumAttr())
            .ToList();
    }

    public async Task<IReadOnlyList<OutUserAlbumAttribute>> FindByUserIdAsync(long userId, int skip = 0, int take = 100, string? filter = null)
    {
        var sql = builder.BuildSelect(null);
        var parameters = new List<NpgsqlParameter>();

        sql += $" WHERE {UserAlbumAttrColumns.UserId} = @userId";
        parameters.Add(new NpgsqlParameter("@userId", userId));

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" AND similarity(al0.{AlbumColumns.Name}, @filter) > 0.4";
            parameters.Add(new NpgsqlParameter("@filter", filter));
        }

        sql += builder.BuildOrderBy($"uaa0.{UserAlbumAttrColumns.UpdatedAt} DESC", $"u0.{UserColumns.Name}");
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToUserAlbumAttr())
            .ToList();
    }

    public async Task<OutUserAlbumAttribute?> FindOneAlbumFromUserAsync(long userId, long albumId)
    {
        var sql = new StringBuilder(builder.BuildSelect(null));
        sql.Append($" WHERE {UserAlbumAttrColumns.UserId} = @userId AND {UserAlbumAttrColumns.AlbumId} = @albumId");

        var parameters = new List<NpgsqlParameter>
        {
            new("@userId", userId),
            new("@albumId", albumId)
        };

        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToUserAlbumAttr();
    }
}