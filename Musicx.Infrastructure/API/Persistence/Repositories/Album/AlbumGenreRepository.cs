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
using Musicx.Infrastructure.API.Persistence.Builders.Core.Commands;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Album;

internal sealed class AlbumGenreRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InAlbumGenre> builder,
    ManyToManySyncService manyToManySync,
    ILoggerProvider loggerProvider) : IAlbumGenreRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumGenreRepository));

    public async Task DeleteOneAsync(long albumId, long genreId, long taggerId)
    {
        const string sql = $"DELETE FROM album_genre " +
                           $"WHERE {AlbumGenreColumns.TaggerId} = @taggerId AND {AlbumGenreColumns.AlbumId} = @albumId AND {AlbumGenreColumns.GenreId} = @genreId";

        var parameters = new List<NpgsqlParameter>
        {
            new("@taggerId", taggerId),
            new("@albumId", albumId),
            new("@genreId", genreId)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

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
        var albumGenres = entities.ToList();

        if (albumGenres.Count == 0)
        {
            return [];
        }
        
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            foreach (var group in albumGenres.GroupBy(x => new { x.AlbumId, x.TaggerId }))
            {
                await manyToManySync.SyncAsync(
                    table: "album_genre",
                    keyColumn: AlbumGenreColumns.AlbumId,
                    targetColumn: AlbumGenreColumns.GenreId,
                    keyValue: group.Key.AlbumId,
                    newValues: group.Select(x => x.GenreId).Distinct().ToList(),
                    connection: conn,
                    transaction: transaction,
                    scopedColumns: new Dictionary<string, object?>
                    {
                        { AlbumGenreColumns.TaggerId, group.Key.TaggerId }
                    }
                );
            }

            foreach (var entity in albumGenres)
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