namespace Musicx.Application.Shared.Utilities;

public static class EnumHelper
{
    public static TEnum ParseOrDefault<TEnum>(string value, TEnum defaultValue) where TEnum : struct, Enum
    {
        return Enum.TryParse<TEnum>(value, out var result) ? result : defaultValue;
    } 
}