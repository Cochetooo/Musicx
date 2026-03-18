using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.User;

internal sealed class UserArtistAttrSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InUserArtistAttribute>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserArtistAttrSqlBuilder));
    
    internal override Task<object?> ExecuteInsert(InUserArtistAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();

    internal override Task ExecuteUpdate(InUserArtistAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();
    
    internal override async Task<object?> ExecuteUpsert(InUserArtistAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var now = DateTime.Now;
        var query = UpsertBuilder.Build(
            table: "user_artist_attrs",
            insertProperties: new Dictionary<string, object?>
            {
                { UserArtistAttrColumns.UserId, entity.UserId },
                { UserArtistAttrColumns.ArtistId, entity.ArtistId },
                { UserArtistAttrColumns.CreatedAt, now },
                { UserArtistAttrColumns.UpdatedAt, now },
                { UserArtistAttrColumns.Follow, entity.Follow },
                { UserArtistAttrColumns.Rating, entity.Rating }
            },
            conflictColumns:
            [
                UserArtistAttrColumns.UserId,
                UserArtistAttrColumns.ArtistId
            ],
            updateProperties: new Dictionary<string, object?>
            {
                { UserArtistAttrColumns.UpdatedAt, now },
                { UserArtistAttrColumns.Follow, entity.Follow },
                { UserArtistAttrColumns.Rating, entity.Rating }
            }
        );

        _logger.LogDebug(SqlHelper.InterpolateQuery(query.Query, query.Parameters));

        await using var cmd = new NpgsqlCommand(query.Query, connection, transaction);
        cmd.Parameters.AddRange(query.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();

        return null;
    }
    
    internal override string BuildSelect(IJoinSpecification<InUserArtistAttribute>? spec = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT uar0.*, u0.*, ar0.* FROM user_artist_attrs uar0 " +
              $"JOIN users u0 ON uar0.{UserArtistAttrColumns.UserId} = u0.{UserColumns.Id} " +
              $"JOIN artists ar0 ON uar0.{UserArtistAttrColumns.ArtistId} = ar0.{ArtistColumns.Id} "
            : "SELECT uar0.*, u0.*, ar0.* FROM user_artist_attrs uar0 " +
              $"JOIN users u0 ON uar0.{UserArtistAttrColumns.UserId} = u0.{UserColumns.Id} " +
              $"JOIN artists ar0 ON uar0.{UserArtistAttrColumns.ArtistId} = ar0.{ArtistColumns.Id} ";

    internal override string BuildGroupBy(IJoinSpecification<InUserArtistAttribute>? spec = null)
        => string.Empty;
}