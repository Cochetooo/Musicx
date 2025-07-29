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

        if (filter is not null)
        {
            sql += $" WHERE ar0.{ArtistColumns.Name} ILIKE @filter";
            parameters.Add(new NpgsqlParameter("@filter", $"%{filter}%"));
        }
        
        sql += $" ORDER BY ar0.{ArtistColumns.Name} OFFSET @skip LIMIT @take";
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
        /*await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                _logger.LogDebug($"📄 SQL : INSERT INTO artists (name, artwork_url, country) " +
                                 $"VALUES ('{entity.Name}', '{entity.ArtworkUrl}', '{entity.Country}')");
                
                //entity.CreatedAt = DateTime.Now;
                //entity.UpdatedAt = DateTime.Now;
                
                //context.Artists.Add(entity);
            }
            else
            {
                _logger.LogDebug($"📄 SQL : UPDATE artists SET Name={entity.Name}, " +
                                 $"ArtworkUrl={entity.ArtworkUrl}, Country={entity.Country}" +
                                 $"WHERE Id = {entity.Id}");
                
                //entity.UpdatedAt = DateTime.Now;
                context.Artists.Update(entity);
                context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return entity.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Artist : Could not persist.", ex, _logger);
        }*/
        throw new NotImplementedException();
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InArtist> entities)
    {
        /*_logger.LogDebug("📄 SAVE ALL Artist");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<long>();

        try
        {
            foreach (var artist in entities)
            {
                if (0 == artist.Id)
                {
                    context.Artists.Add(artist);
                }
                else
                {
                    context.Artists.Update(artist);
                }
                
                ids.Add(artist.Id);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ids;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Artist : Could not persist", ex, _logger);
        }*/
        
        throw new NotImplementedException();
    }

    private static string Select(IQuerySpecification<InArtist>? querySpecification = null)
    {
        if (querySpecification is not ArtistQuerySpecification artistQuerySpecification)
        {
            return "SELECT ar0.* FROM artists ar0";
        }

        return "SELECT ar0.* FROM artists ar0";
    }
}