using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

public sealed class GenreSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InGenre>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreSqlBuilder));
    
    internal override Task<object?> ExecuteInsert(InGenre entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override Task ExecuteUpdate(InGenre entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IQuerySpecification<InGenre>? querySpecification = null)
    {
        if (querySpecification is not GenreQuerySpecification genreQuerySpecification)
        {
            return "SELECT g0.* FROM genres g0";
        }

        var selects = new List<string> { "g0.*" };
        var joins = new List<string>();

        if (genreQuerySpecification.IncludeChildren)
        {
            selects.Add($"json_agg(cg.*) FILTER (WHERE cg.{GenreColumns.Id} IS NOT NULL) AS children");
            joins.Add($"LEFT JOIN childrengenre_parentgenre cgpg ON g0.{GenreColumns.Id} = cgpg.{ChildrenGenreParentGenreColumns.ParentId}");
            joins.Add($"LEFT JOIN genres cg ON cgpg.{ChildrenGenreParentGenreColumns.ChildId} = cg.{GenreColumns.Id}");
        }
        
        if (genreQuerySpecification.IncludeParents)
        {
            selects.Add($"json_agg(pg.*) FILTER (WHERE pg.{GenreColumns.Id} IS NOT NULL) AS parents");
            joins.Add($"LEFT JOIN childrengenre_parentgenre pgcg ON g0.{GenreColumns.Id} = pgcg.{ChildrenGenreParentGenreColumns.ChildId}");
            joins.Add($"LEFT JOIN genres pg ON pgcg.{ChildrenGenreParentGenreColumns.ParentId} = pg.{GenreColumns.Id}");
        }

        return $"SELECT {string.Join(", ", selects)} FROM genres g0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IQuerySpecification<InGenre>? spec = null)
        => $" GROUP BY g0.{GenreColumns.Id}";
}