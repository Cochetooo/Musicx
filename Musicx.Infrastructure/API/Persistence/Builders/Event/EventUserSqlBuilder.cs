using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Event;
using Musicx.Infrastructure.API.Persistence.Columns.Event;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Event;

internal sealed class EventUserSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InEventUser>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(EventUserSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InEventUser entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = BuildInsert("event_user",
            new Dictionary<string, object?>
            {
                { EventUserColumns.UserId, entity.UserId },
                { EventUserColumns.EventId, entity.EventId },
                { EventUserColumns.Comment, entity.Comment },
                { EventUserColumns.IsGoing, entity.IsGoing },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
        
        return null;
    }

    internal override async Task ExecuteUpdate(InEventUser entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("event_user",
            [EventUserColumns.EventId, EventUserColumns.UserId],
            [entity.EventId, entity.UserId],
            new Dictionary<string, object?>
            {
                { EventUserColumns.Comment, entity.Comment },
                { EventUserColumns.IsGoing, entity.IsGoing },
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteScalarAsync();
    }

    internal override Task ExecuteUpsert(InEventUser entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IJoinSpecification<InEventUser>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT evar0.*, ev0.*, u0.* FROM event_user evar0 " + 
              $"INNER JOIN users u0 ON evar0.{EventUserColumns.UserId} = u0.{UserColumns.Id} " +
              $"INNER JOIN events ev0 ON evar0.{EventUserColumns.EventId} = ev0.{EventColumns.Id} "
            : "SELECT evar0.*, ev0.*, u0.* FROM event_user evar0 " + 
              $"INNER JOIN users u0 ON evar0.{EventUserColumns.UserId} = u0.{UserColumns.Id} " +
              $"INNER JOIN events ev0 ON evar0.{EventUserColumns.EventId} = ev0.{EventColumns.Id} ";

    internal override string BuildGroupBy(IJoinSpecification<InEventUser>? querySpecification = null)
        => string.Empty;
}