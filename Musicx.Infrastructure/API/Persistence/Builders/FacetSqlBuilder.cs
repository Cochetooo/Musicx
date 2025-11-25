using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class FacetSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InFacet>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FacetSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InFacet entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long facetId;

        var createCommandSql = BuildInsert("facets",
            new Dictionary<string, object?>
            {
                { FacetColumns.CreatedAt, DateTime.Now },
                { FacetColumns.UpdatedAt, DateTime.Now },
                { FacetColumns.Description, entity.Description },
                { FacetColumns.Name, entity.Name },
                { FacetColumns.Type, entity.Type },
            },
            returningColumn: FacetColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            facetId = (long) (await cmd.ExecuteScalarAsync() ??
                              throw new NullReferenceException("Could not insert entity"));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + facetId);

        return facetId;
    }

    internal override async Task ExecuteUpdate(InFacet entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("facets",
            FacetColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { FacetColumns.UpdatedAt, DateTime.Now },
                { FacetColumns.Description, entity.Description },
                { FacetColumns.Name, entity.Name },
                { FacetColumns.Type, entity.Type },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteScalarAsync();
    }

    internal override string BuildSelect(IQuerySpecification<InFacet>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT fa0.* FROM facets fa0 "
            : "SELECT fa0.* FROM facets fa0 ";

    internal override string BuildGroupBy(IQuerySpecification<InFacet>? querySpecification = null)
        => string.Empty;
}