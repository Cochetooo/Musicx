using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class ArtistRepository(
    IDbConnectionProvider connection,
    ILoggerProvider loggerProvider) : IArtistRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM artists WHERE {ArtistColumns.Id} = @id";
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM artists WHERE {ArtistColumns.Id} IN (@ids)";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutArtist?> FindByIdAsync(long id, IQuerySpecification<InArtist>? artistQuerySpecification = null)
    {
        var sql = new StringBuilder(Select(artistQuerySpecification));
        sql.Append($" WHERE ar0.{ArtistColumns.Id} = @id");
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToArtist();
    }

    public async Task<List<OutArtist>> FindAsync(int skip = 0, int take = 100,
        IQuerySpecification<InArtist>? artistQuerySpecification = null, string? filter = null)
    {
        var sql = Select(artistQuerySpecification);
        
        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" WHERE similarity(ar0.{ArtistColumns.Name}, @filter) > 0.4";
            sql += $" ORDER BY similarity(ar0.{ArtistColumns.Name}, @filter) DESC";
            
            parameters.Add(new NpgsqlParameter("@filter", filter));
        }
        else
        {
            sql += $" ORDER BY ar0.{ArtistColumns.Name}";
        }
        
        sql +=  " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToArtist())
            .ToList();
    }

    public async Task<List<OutArtist>> FindIn(IEnumerable<long> ids, IQuerySpecification<InArtist>? artistQuerySpecification = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }

        var stringIds = string.Join(",", idList);
        var sql = Select(artistQuerySpecification);
        sql += $" WHERE ar0.{ArtistColumns.Id} IN ({stringIds})" +
               $" ORDER BY ar0.{ArtistColumns.Name}";

        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToArtist())
            .ToList();
    }

    public async Task<int> GetCountAsync()
        => await connection.Count("artists");

    public async Task<long> SaveAsync(InArtist entity)
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
                await Update(entity, conn, transaction);
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Artist : Could not persist.", ex, _logger);
        }

        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InArtist> entities)
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
                    await Update(entity, conn, transaction);
                }
                
                idList.Add(entity.Id);
            }
            
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Artist : Could not persist.", ex, _logger);
        }
        
        return idList;
    }

    private static string Select(IQuerySpecification<InArtist>? querySpecification = null)
    {
        if (querySpecification is not ArtistQuerySpecification artistQuerySpecification)
        {
            return "SELECT ar0.* FROM artists ar0";
        }

        return "SELECT ar0.* FROM artists ar0";
    }

    private async Task<long> Insert(InArtist entity, NpgsqlConnection conn, NpgsqlTransaction transaction)
    {
        var createCommandSql = SqlHelper.Insert("artists",
            new Dictionary<string, object?>
            {
                { ArtistColumns.CreatedAt, DateTime.Now },
                { ArtistColumns.UpdatedAt, DateTime.Now },
                { ArtistColumns.ArtworkUrl, entity.ArtworkUrl },
                { ArtistColumns.Country, entity.Country },
                { ArtistColumns.Description, entity.Description },
                { ArtistColumns.Name, entity.Name },
                { ArtistColumns.Region, entity.Region },
                { ArtistColumns.Town, entity.Town },
                { ArtistColumns.Discriminator, entity.Discriminator },
                { ArtistColumns.FormationDate, entity.FormationDate },
                { ArtistColumns.SplitDate, entity.SplitDate },
                { ArtistColumns.FirstName, entity.FirstName },
                { ArtistColumns.LastName, entity.LastName },
                { ArtistColumns.BirthDate, entity.BirthDate },
                { ArtistColumns.DeathDate, entity.DeathDate }
            }, ArtistColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Item1, createCommandSql.Item2));

        await using var cmd = new NpgsqlCommand(createCommandSql.Item1, conn, transaction);
        cmd.Parameters.AddRange(createCommandSql.Item2.ToArray());
        var artistId = (long)(await cmd.ExecuteScalarAsync() ??
                              throw new NullReferenceException("Could not insert entity."));

        return artistId;
    }
    
    private async Task Update(InArtist entity, NpgsqlConnection conn, NpgsqlTransaction transaction)
    {
        var createCommandSql = SqlHelper.Update("artists",
            ArtistColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { ArtistColumns.UpdatedAt, DateTime.Now },
                { ArtistColumns.ArtworkUrl, entity.ArtworkUrl },
                { ArtistColumns.Country, entity.Country },
                { ArtistColumns.Description, entity.Description },
                { ArtistColumns.Name, entity.Name },
                { ArtistColumns.Region, entity.Region },
                { ArtistColumns.Town, entity.Town },
                { ArtistColumns.Discriminator, entity.Discriminator },
                { ArtistColumns.FormationDate, entity.FormationDate },
                { ArtistColumns.SplitDate, entity.SplitDate },
                { ArtistColumns.FirstName, entity.FirstName },
                { ArtistColumns.LastName, entity.LastName },
                { ArtistColumns.BirthDate, entity.BirthDate },
                { ArtistColumns.DeathDate, entity.DeathDate }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Item1, createCommandSql.Item2));

        await using var cmd = new NpgsqlCommand(createCommandSql.Item1, conn, transaction);
        cmd.Parameters.AddRange(createCommandSql.Item2.ToArray());
        await cmd.ExecuteScalarAsync();
    }
}