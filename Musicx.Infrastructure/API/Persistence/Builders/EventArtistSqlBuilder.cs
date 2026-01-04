using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class EventArtistSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InEventArtist>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(EventArtistSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InEventArtist entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = BuildInsert("event_artist",
            new Dictionary<string, object?>
            {
                { EventArtistColumns.ArtistId, entity.ArtistId },
                { EventArtistColumns.EventId, entity.EventId },
                { EventArtistColumns.BeginDate, entity.BeginDate },
                { EventArtistColumns.EndDate, entity.EndDate },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
        
        return null;
    }

    internal override async Task ExecuteUpdate(InEventArtist entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("event_artist",
            [EventArtistColumns.EventId, EventArtistColumns.ArtistId],
            [entity.EventId, entity.ArtistId],
            new Dictionary<string, object?>
            {
                { EventArtistColumns.BeginDate, entity.BeginDate },
                { EventArtistColumns.EndDate, entity.EndDate },
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteScalarAsync();
    }

    internal override Task ExecuteUpsert(InEventArtist entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IQuerySpecification<InEventArtist>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT evar0.*, ev0.*, ar0.* FROM event_artist evar0 " + 
              $"INNER JOIN artists ar0 ON evar0.{EventArtistColumns.ArtistId} = ar0.{ArtistColumns.Id} " +
              $"INNER JOIN events ev0 ON evar0.{EventArtistColumns.EventId} = ev0.{EventColumns.Id} "
            : "SELECT evar0.*, ev0.*, ar0.* FROM event_artist evar0 " + 
              $"INNER JOIN artists ar0 ON evar0.{EventArtistColumns.ArtistId} = ar0.{ArtistColumns.Id} " +
              $"INNER JOIN events ev0 ON evar0.{EventArtistColumns.EventId} = ev0.{EventColumns.Id} ";

    internal override string BuildGroupBy(IQuerySpecification<InEventArtist>? querySpecification = null)
        => string.Empty;
}