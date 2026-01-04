using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class LabelSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InLabel>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(LabelSqlBuilder));

    internal override async Task<object?> ExecuteInsert(InLabel entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        long labelId;

        var createCommandSql = BuildInsert("labels",
            new Dictionary<string, object?>
            {
                { LabelColumns.CreatedAt, DateTime.Now },
                { LabelColumns.UpdatedAt, DateTime.Now },
                { LabelColumns.Description, entity.Description },
                { LabelColumns.IsVisible, entity.IsVisible },
                { LabelColumns.Name, entity.Name }
            },
            returningColumn: LabelColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            labelId = (long) (await cmd.ExecuteScalarAsync() ??
                                  throw new NullReferenceException("Could not insert entity"));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + labelId);

        if (entity.ReleaseIds.Count > 0)
        {
            foreach (var release in entity.ReleaseIds)
            {
                
            }
        }

        return labelId;
    }

    internal override async Task ExecuteUpdate(InLabel entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("labels",
            GenreAliasColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { LabelColumns.UpdatedAt, DateTime.Now },
                { LabelColumns.Description, entity.Description },
                { LabelColumns.IsVisible, entity.IsVisible },
                { LabelColumns.Name, entity.Name }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();
    }

    internal override Task ExecuteUpsert(InLabel entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IQuerySpecification<InLabel>? querySpecification = null,
        bool distinct = false)
    {
        if (querySpecification is not LabelQuerySpecification labelQuerySpecification)
        {
            return distinct
                ? "SELECT DISTINCT l0.* FROM labels l0"
                : "SELECT l0.* FROM labels l0";
        }

        var selects = new List<string> { "l0.*" };
        var joins = new List<string>();

        if (labelQuerySpecification.IncludeReleases)
        {
            //selects.Add("rel0.*");
            //joins.Add($"JOIN releases rel0 ON l0.{}");
        }
        
        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM labels l0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM labels l0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IQuerySpecification<InLabel>? spec = null)
        => $" GROUP BY l0.{LabelColumns.Id}";
}