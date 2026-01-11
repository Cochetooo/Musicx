using System.Reflection;
using System.Web;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Infrastructure.Web.Helpers;

public static class QueryStringHelper
{
    public static string SetUseCaseParameters<T>(
        IJoinSpecification<T>? joinSpec = null,
        OrderSpecification<T>? orderSpec = null, 
        PagingOptions? pagingOptions = null
    )
        where T : BaseInputModel
    {
        var parameters = new List<string>();

        if (joinSpec is not null)
        {
            var props = joinSpec.GetType().GetProperties();

            foreach (var prop in props)
            {
                var value = prop.GetValue(joinSpec);
                if (value is not bool b || b == false)
                {
                    continue;
                }

                var name = prop.Name.ToCamelCase();
                parameters.Add($"joins.{name}=true");
            }
        }

        if (orderSpec is not null)
        {
            var props = orderSpec.GetType().GetProperties();

            foreach (var prop in props)
            {
                var value = prop.GetValue(orderSpec);
                if (value is null)
                {
                    continue;
                }

                var name = prop.Name.ToCamelCase();
                parameters.Add($"order.{name}={value}");
            }
        }

        if (pagingOptions is not null)
        {
            parameters.Add($"paging.take={pagingOptions.Take}");
            parameters.Add($"paging.skip={pagingOptions.Skip}");
        }
        
        return string.Join("&", parameters);
    }
    
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
        return string.Concat(
            input.Select((x, i) =>
                i > 0 && char.IsUpper(x)
                    ? "_" + char.ToLower(x)
                    : char.ToLower(x).ToString()
            )
        );
    }
    
    private static string ToCamelCase(this string name)
    {
        return char.ToLowerInvariant(name[0]) + name[1..];
    }
}