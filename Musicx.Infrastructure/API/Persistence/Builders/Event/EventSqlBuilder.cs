using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Event;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.Event;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Specifications.Event;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Event;

internal sealed class EventSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InEvent>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(EventSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InEvent entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long eventId;

        var createCommandSql = BuildInsert("events",
            new Dictionary<string, object?>
            {
                { EventColumns.CreatedAt, DateTime.Now },
                { EventColumns.UpdatedAt, DateTime.Now },
                { EventColumns.Prices, entity.EventPrices },
                { EventColumns.TicketLinks, entity.EventTicketLinks },
                { EventColumns.Address, entity.Address },
                { EventColumns.BeginDate, entity.BeginDate },
                { EventColumns.Country, entity.Country },
                { EventColumns.Description, entity.Description },
                { EventColumns.EndDate, entity.EndDate },
                { EventColumns.IsFestival, entity.IsFestival },
                { EventColumns.IsVisible, entity.IsVisible },
                { EventColumns.Name, entity.Name },
                { EventColumns.PosterUrl, entity.PosterUrl },
                { EventColumns.Town, entity.Town },
                { EventColumns.Venue, entity.Venue },
                { EventColumns.ZipCode, entity.ZipCode }
            },
            returningColumn: EventColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            eventId = (long) (await cmd.ExecuteScalarAsync() ??
                              throw new NullReferenceException("Could not insert entity"));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + eventId);

        return eventId;
    }

    internal override async Task ExecuteUpdate(InEvent entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("events",
            EventColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { EventColumns.UpdatedAt, DateTime.Now },
                { EventColumns.Prices, entity.EventPrices },
                { EventColumns.TicketLinks, entity.EventTicketLinks },
                { EventColumns.Address, entity.Address },
                { EventColumns.BeginDate, entity.BeginDate },
                { EventColumns.Country, entity.Country },
                { EventColumns.Description, entity.Description },
                { EventColumns.EndDate, entity.EndDate },
                { EventColumns.IsFestival, entity.IsFestival },
                { EventColumns.IsVisible, entity.IsVisible },
                { EventColumns.Name, entity.Name },
                { EventColumns.PosterUrl, entity.PosterUrl },
                { EventColumns.Town, entity.Town },
                { EventColumns.Venue, entity.Venue },
                { EventColumns.ZipCode, entity.ZipCode }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteScalarAsync();
    }

    internal override Task ExecuteUpsert(InEvent entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IJoinSpecification<InEvent>? querySpecification = null, bool distinct = false)
    {
        if (querySpecification is not EventJoinSpecification eventQuerySpecification)
        {
            return distinct 
                ? "SELECT DISTINCT e0.* FROM events e0"
                : "SELECT e0.* FROM events e0";
        }

        var selects = new List<string> { "e0.*" };
        var joins = new List<string>();

        if (eventQuerySpecification.IncludeArtists)
        {
            selects.Add($"(SELECT json_agg(ea.*, ar0.*) FROM event_artist ea " +
                        $"INNER JOIN artists ar0 ON ea.{EventArtistColumns.ArtistId} = ar0.{ArtistColumns.Id} " +
                        $"WHERE e0.{EventColumns.Id} = ea.{EventArtistColumns.EventId}) AS event_artists");
        }
        
        if (eventQuerySpecification.IncludeUsers)
        {
            selects.Add($"(SELECT json_agg(eu.*, u0.*) FROM event_user eu " +
                        $"INNER JOIN users u0 ON eu.{EventUserColumns.UserId} = u0.{UserColumns.Id} " +
                        $"WHERE e0.{EventColumns.Id} = eu.{EventUserColumns.EventId}) AS event_users");
        }

        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM events e0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM events e0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IJoinSpecification<InEvent>? querySpecification = null)
    {
        if (querySpecification is not EventJoinSpecification eventQuerySpecification)
        {
            return $" GROUP BY e0.{EventColumns.Id}";
        }

        var groupings = new List<string>
        {
            $"e0.{EventColumns.Id}"
        };

        if (eventQuerySpecification.IncludeArtists)
        {
            groupings.Add($"ar0.{ArtistColumns.Id}");
        }

        if (eventQuerySpecification.IncludeUsers)
        {
            groupings.Add($"u0.{UserColumns.Id}");
        }
        
        return $" GROUP BY {string.Join(", ", groupings)}";
    }
}