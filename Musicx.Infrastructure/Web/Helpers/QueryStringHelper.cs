using System.Reflection;
using System.Web;

namespace Musicx.Infrastructure.Web.Helpers;

public static class QueryStringHelper
{
    public static string ToQueryString(this object obj)
    {
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var query = HttpUtility.ParseQueryString(string.Empty);
        
        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);
            if (value == null) continue;

            // Si c’est un tableau => on rejoint par des virgules
            if (value is IEnumerable<string> stringArray)
                query[prop.Name] = string.Join(",", stringArray);

            else if (value is IEnumerable<long> longArray)
                query[prop.Name] = string.Join(",", longArray);
            
            else if (value is bool b)
                query[prop.Name] = b.ToString().ToLower();

            else if (value is DateTime d)
                query[prop.Name] = d.ToString("yyyy-MM-dd");

            else
                query[prop.Name] = value.ToString();
        }

        return "?" + query.ToString();
    }
    
    private static string ToSnakeCase(this string input)
    {
        // facultatif : convertir les noms en snake_case pour ton API
        return string.Concat(
            input.Select((x, i) =>
                i > 0 && char.IsUpper(x)
                    ? "_" + char.ToLower(x)
                    : char.ToLower(x).ToString()
            )
        );
    }
}