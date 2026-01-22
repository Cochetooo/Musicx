using System.Data.Common;
using Npgsql;

namespace Musicx.Infrastructure.Shared.Helpers;

public static class SqlHelper
{
    public static string InterpolateQuery(string rawSql, IEnumerable<DbParameter> parameters)
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
}