using System.ComponentModel;
using System.Globalization;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Builders.Core;
using Musicx.Infrastructure.API.Persistence.Builders.Core.Commands;
using Musicx.Infrastructure.API.Persistence.Builders.Core.Query;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

/// <summary>
/// Base abstract class responsible for generating
/// SQL statements (INSERT / UPDATE / UPSERT / SELECT)
/// for a given input model type.
/// 
/// It centralizes low-level SQL generation logic in a
/// reusable and secure way (parameterized queries).
/// </summary>
/// <typeparam name="T">The input model type.</typeparam>
internal abstract class SqlBuilder<T> where T : BaseInputModel
{
    protected readonly InsertBuilder InsertBuilder = new();
    protected readonly UpdateBuilder UpdateBuilder = new();
    protected readonly UpsertBuilder UpsertBuilder = new();
    protected readonly FilterBuilder FilterBuilder = new();
    
    /// <summary>
    /// Executes an INSERT operation for the given entity.
    /// </summary>
    internal abstract Task<object?> ExecuteInsert(T entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    /// <summary>
    /// Executes an UPDATE operation for the given entity.
    /// </summary>
    internal abstract Task ExecuteUpdate(T entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    /// <summary>
    /// Executes and UPSERT operation for the given entity.
    /// </summary>
    internal abstract Task ExecuteUpsert(T entity,
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    /// <summary>
    /// Builds a SELECT query with a given specification for joins.
    /// </summary>
    internal abstract string BuildSelect(IJoinSpecification<T>? spec = null, bool distinct = false);

    internal virtual (string Sql, List<NpgsqlParameter> Parameters) BuildFilteredQuery(
        IFindQuery<T> query,
        IJoinSpecification<T>? joinSpec,
        OrderSpecification<T>? orderSpec,
        PagingOptions? pagingOptions,
        bool countOnly) => (string.Empty, []);
    
    /// <summary>
    /// Builds a standard filtered query for entities with a unique identifier.
    /// </summary>
    /// <param name="query">Typed find query.</param>
    /// <param name="joinSpec">Optional join specification.</param>
    /// <param name="orderSpec">Optional ordering specification.</param>
    /// <param name="pagingOptions">Optional paging options.</param>
    /// <param name="countOnly">When true, only emits COUNT SQL.</param>
    /// <param name="searchColumns">Columns targeted by raw search.</param>
    /// <param name="defaultOrderColumn">Default order column when no order spec is provided.</param>
    /// <returns>SQL string and associated parameters.</returns>
    protected (string Sql, List<NpgsqlParameter> Parameters) BuildDefaultFilteredQuery(
        IFindQuery<T> query,
        IJoinSpecification<T>? joinSpec,
        OrderSpecification<T>? orderSpec,
        PagingOptions? pagingOptions,
        bool countOnly,
        IReadOnlyCollection<string> searchColumns,
        string defaultOrderColumn)
    {
        var sql = BuildSelect(joinSpec);
        var parameters = new List<NpgsqlParameter>();

        if (query.RawSearch is not null && searchColumns.Count > 0)
        {
            Filter(
                ref sql,
                searchColumns,
                query.RawSearch.Value ?? string.Empty,
                parameters,
                query.Search?.Exact ?? false,
                query.Search?.Similarity ?? 0.4
            );
        }

        sql += BuildGroupBy(joinSpec);

        if (countOnly)
        {
            return ($"SELECT COUNT(*) FROM ({sql}) q0", parameters);
        }

        if (!countOnly)
        {
            if (orderSpec is not null)
            {
                sql += BuildOrderBy(orderSpec);
            }
            else
            {
                sql += !string.IsNullOrWhiteSpace(query.RawSearch?.Value)
                    ? $" ORDER BY similarity({defaultOrderColumn}, @filter) DESC"
                    : $" ORDER BY {defaultOrderColumn}";
            }

            sql += " OFFSET @skip LIMIT @take";
            parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
            parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));
        }

        return (sql, parameters);
    }

    /// <summary>
    /// Builds a GROUP BY clause with a given specification for joins.
    /// </summary>
    internal abstract string BuildGroupBy(IJoinSpecification<T>? spec = null);

    /// <summary>
    /// Builds an ORDER BY clause from an OrderSpecification.
    /// 
    /// Returns empty string if no ordering defined.
    /// </summary>
    internal string BuildOrderBy(OrderSpecification<T> orderSpec)
    {
        // Validate specification before generating SQL
        orderSpec.Validate();

        // Convert specification into SQL fragments
        var clauses = orderSpec.ToClauses();

        if (!clauses.Any())
        {
            return string.Empty;
        }

        return $" ORDER BY {string.Join(", ", orderSpec.ToClauses().Select(c => c.Field + " " + c.Direction + (c.NullLast ? " NULLS LAST" : string.Empty)))}";
    }

    /// <summary>
    /// Adds a WHERE clause for text filtering.
    /// 
    /// Supports:
    /// - Exact ILIKE match
    /// - Approximate match using PostgreSQL similarity()
    /// 
    /// WARNING:
    /// This method appends a WHERE clause directly.
    /// Ensure no previous WHERE clause exists.
    /// </summary>
    internal void Filter(ref string sql, IEnumerable<string> columns,
        string filter, List<NpgsqlParameter> parameters,
        bool? filterExact = null, double? filterSimilitude = null)
        => FilterBuilder.Filter(ref sql, columns, filter, parameters, filterExact, filterSimilitude);
    
    internal void Filter(ref string sql, string column, string filter, List<NpgsqlParameter> parameters,
        bool? filterExact = null, double? filterSimilitude = null)
        => FilterBuilder.Filter(ref sql, column, filter, parameters, filterExact, filterSimilitude);
}