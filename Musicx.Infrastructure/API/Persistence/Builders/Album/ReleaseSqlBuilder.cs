using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;
using Musicx.Infrastructure.API.Persistence.Columns.Label;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Album;

internal sealed class ReleaseSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InRelease>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ReleaseSqlBuilder));

    internal override async Task<object?> ExecuteInsert(InRelease entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        long releaseId;

        var createCommandSql = BuildInsert("releases",
            new Dictionary<string, object?>
            {
                { ReleaseColumns.AlbumId, entity.AlbumId },
                { ReleaseColumns.LabelId, entity.LabelId },
                { ReleaseColumns.CreatedAt, DateTime.Now },
                { ReleaseColumns.UpdatedAt, DateTime.Now },
                { ReleaseColumns.CatalogNumber, entity.CatalogNumber },
                { ReleaseColumns.IsVisible, entity.IsVisible },
                { ReleaseColumns.ReleaseDate, entity.ReleaseDate }
            },
            returningColumn: ReleaseColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            releaseId = (long) (await cmd.ExecuteScalarAsync() ??
                                  throw new NullReferenceException("Could not insert entity"));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + releaseId);

        return releaseId;
    }

    internal override async Task ExecuteUpdate(InRelease entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("releases",
            GenreAliasColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { ReleaseColumns.UpdatedAt, DateTime.Now },
                { ReleaseColumns.CatalogNumber, entity.CatalogNumber },
                { ReleaseColumns.IsVisible, entity.IsVisible },
                { ReleaseColumns.ReleaseDate, entity.ReleaseDate }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();
    }

    internal override Task ExecuteUpsert(InRelease entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IJoinSpecification<InRelease>? querySpecification = null,
        bool distinct = false)
    {
        if (querySpecification is not ReleaseJoinSpecification releaseQuerySpecification)
        {
            return distinct
                ? "SELECT DISTINCT r0.* FROM releases r0"
                : "SELECT r0.* FROM releases r0";
        }

        var selects = new List<string> { "r0.*" };
        var joins = new List<string>();

        if (releaseQuerySpecification.IncludeAlbum)
        {
            selects.Add("al0.*");
            joins.Add($"JOIN albums al0 ON r0.{ReleaseColumns.AlbumId} = al0.{AlbumColumns.Id}");
        }

        if (releaseQuerySpecification.IncludeLabel)
        {
            selects.Add("l0.*");
            joins.Add($"JOIN labels l0 ON r0.{ReleaseColumns.LabelId} = l0.{LabelColumns.Id}");
        }
        
        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM releases r0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM releases r0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IJoinSpecification<InRelease>? spec = null)
        => $" GROUP BY r0.{ReleaseColumns.Id}";
}