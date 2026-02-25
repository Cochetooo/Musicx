using System.Text;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core;

internal sealed class SqlQueryComposer(string baseSql)
{
    private readonly StringBuilder _sql = new(baseSql);
    private readonly List<NpgsqlParameter> _parameters = new();

    /// <summary>
    /// Adds a WHERE condition.
    /// If a WHERE already exists, appends with AND.
    /// </summary>
    public void AddCondition(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return;

        if (!_sql.ToString().Contains("WHERE", StringComparison.OrdinalIgnoreCase))
        {
            _sql.Append(" WHERE ");
        }
        else
        {
            _sql.Append(" AND ");
        }

        _sql.Append(condition);
    }

    /// <summary>
    /// Adds raw SQL (ORDER BY, GROUP BY, etc).
    /// </summary>
    public void Append(string sqlFragment)
    {
        if (string.IsNullOrWhiteSpace(sqlFragment))
            return;

        _sql.Append(" ");
        _sql.Append(sqlFragment);
    }

    /// <summary>
    /// Adds a parameter to the statement.
    /// </summary>
    public void AddParameter(NpgsqlParameter parameter)
    {
        _parameters.Add(parameter);
    }

    /// <summary>
    /// Adds multiple parameters.
    /// </summary>
    public void AddParameters(IEnumerable<NpgsqlParameter> parameters)
    {
        _parameters.AddRange(parameters);
    }

    /// <summary>
    /// Builds the final SqlStatement.
    /// </summary>
    public SqlStatement Build()
    {
        return new SqlStatement(
            _sql.ToString(),
            _parameters
        );
    }
}