using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal record SqlQuery (
    string Query,
    List<NpgsqlParameter> Parameters);

internal abstract class SqlBuilder<T> where T : BaseInputModel
{
    internal abstract Task<object?> ExecuteInsert(T entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    internal abstract Task ExecuteUpdate(T entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    internal abstract string BuildSelect(IQuerySpecification<T>? spec = null);
    
    internal abstract string BuildGroupBy(IQuerySpecification<T>? spec = null);

    /// <summary>
    /// Create a INSERT INTO command with a set of values.
    /// </summary>
    /// <param name="table">The database table name</param>
    /// <param name="properties">A dictionary of properties with its column name and its value</param>
    /// <param name="returningColumn">Optional column to return</param>
    /// <returns></returns>
    internal SqlQuery BuildInsert(string table,
        IDictionary<string, object?> properties,
        string returningColumn = "")
    {
        var keys = new List<string>();
        var values = new List<string>();
        var parameters = new List<NpgsqlParameter>();

        foreach (var property in properties)
        {
            keys.Add(property.Key);
            
            var securizedValue = "@" + property.Key;
            
            values.Add(securizedValue);

            if (property.Value is null)
            {
                parameters.Add(new NpgsqlParameter(securizedValue, DBNull.Value));
            }
            else
            {
                var actualType = Nullable.GetUnderlyingType(property.Value.GetType()) ?? property.Value.GetType();

                if (actualType.IsEnum)
                {
                    if (property.Key.Contains("Discriminator", StringComparison.InvariantCultureIgnoreCase))
                    {
                        parameters.Add(new NpgsqlParameter(securizedValue, property.Value.ToString()));
                    }
                    else
                    {
                        parameters.Add(new NpgsqlParameter(securizedValue, (int)property.Value));
                    }
                }
                else
                {
                    parameters.Add(new NpgsqlParameter(securizedValue, property.Value));
                }
            }
        }
        
        var sql = $"INSERT INTO {table} ({string.Join(", ", keys)}) VALUES ({string.Join(", ", values)})"
                  + (string.IsNullOrWhiteSpace(returningColumn)
                      ? ""
                      : " RETURNING " + returningColumn);
        
        return new SqlQuery(sql, parameters);
    }

    internal SqlQuery BuildUpdate(string table, string whereColumn, long whereId, 
        IDictionary<string, object?> properties)
    {
        var setters = new List<string>();
        var parameters = new List<NpgsqlParameter>();

        foreach (var property in properties)
        {
            if (property.Key == "Id")
            {
                continue;
            }
            
            var securizedValue = "@" + property.Key;
            setters.Add($"{property.Key} = {securizedValue}");

            if (property.Value is null)
            {
                parameters.Add(new NpgsqlParameter(securizedValue, DBNull.Value));
            }
            else
            {
                var actualType = Nullable.GetUnderlyingType(property.Value.GetType()) ?? property.Value.GetType();

                if (actualType.IsEnum)
                {
                    if (property.Key.Contains("Discriminator", StringComparison.InvariantCultureIgnoreCase))
                    {
                        parameters.Add(new NpgsqlParameter(securizedValue, property.Value.ToString()));
                    }
                    else
                    {
                        parameters.Add(new NpgsqlParameter(securizedValue, (int)property.Value));
                    }
                }
                else
                {
                    parameters.Add(new NpgsqlParameter(securizedValue, property.Value));
                }
            }
        }

        var sql = $"UPDATE {table} SET {string.Join(", ", setters)} WHERE {whereColumn} = @Id";
        parameters.Add(new NpgsqlParameter("@Id", whereId));
        
        return new SqlQuery(sql, parameters);
    }
    
    internal string BuildOrderBy(params string[] columns)
        => " ORDER BY " + string.Join(", ", columns);
}