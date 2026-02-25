using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core;

internal static class SqlParameterFactory
{
    public static NpgsqlParameter Create(string name, object? value)
    {
        var paramName = name.StartsWith("@") ? name : "@" + name;

        if (value is null)
            return new NpgsqlParameter(paramName, DBNull.Value);

        var actualType = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();

        if (actualType.IsEnum)
        {
            return new NpgsqlParameter(
                paramName,
                name.Contains("Discriminator", StringComparison.InvariantCultureIgnoreCase)
                    ? value.ToString()
                    : (int)value
            );
        }

        return new NpgsqlParameter(paramName, value);
    }
}