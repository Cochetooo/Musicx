using System.Text.RegularExpressions;

namespace Musicx.Application.Shared.Helpers;

public static class StringHelper
{
    public static int LevenshteinDistance(string source, string target)
    {
        var n = source.Length;
        var m = target.Length;
        var d = new int[n + 1, m + 1];
        
        for (var i = 0; i <= n; d[i, 0] = i++) { }
        for (var j = 0; j <= m; d[0, j] = j++) { }

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                var cost = target[j - 1] == source[i - 1] ? 0 : 1;
                d[i, j] = new[]
                {
                    d[i - 1, j] + 1,
                    d[i, j - 1] + 1,
                    d[i - 1, j - 1] + cost,
                }.Min();
            }
        }
        
        return d[n, m];
    }
    
    public static string Normalize(string? value)
        => string.Join(' ', (value ?? string.Empty)
            .ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : ' ')
            .ToString()?
            .Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? []);
    
    public static string SplitCamelCase(this string input) =>
        Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    
    public static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || char.IsLower(value[0]))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}