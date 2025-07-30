using Npgsql;

namespace Musicx.Infrastructure.Shared.Helpers;

public static class SqlHelper
{
    public static string InterpolateQuery(string rawSql, IEnumerable<NpgsqlParameter> parameters)
    {
        var interpolated = rawSql;

        foreach (var param in parameters)
        {
            var value = param.Value switch
            {
                null => "NULL",
                string s => $"'{s.Replace("'", "''")}'",
                DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'",
                DateTimeOffset dto => $"'{dto:yyyy-MM-dd HH:mm:ss zzz}'",
                bool b => b ? "true" : "false",
                Guid g => $"'{g}'",
                Enum e => Convert.ToInt32(e).ToString(),
                _ => param.Value.ToString()
            };
            
            interpolated = interpolated.Replace(param.ParameterName, value);
        }
        
        return "📜 SQL : " + interpolated;
    }

    public static (string, List<NpgsqlParameter>) Insert(string table, 
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
        
        return (sql, parameters);
    }
    
    public static (string, List<NpgsqlParameter>) Update(string table, 
        string whereColumn, long whereId,
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
        
        return (sql, parameters);
    }
}