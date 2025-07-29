using System.Text.Json;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public sealed class DbToOutModelPolicy : JsonNamingPolicy
{
    private readonly string _prefixToTrim;

    public DbToOutModelPolicy(string prefixToTrim)
    {
        _prefixToTrim = prefixToTrim;
    }

    public override string ConvertName(string name)
    {
        if (name.StartsWith(_prefixToTrim + "_"))
        {
            name = name[(_prefixToTrim.Length+1)..];
        }

        var result = string.Concat(
            name.Split('_', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s[1..]));

        return result;
    }
}