namespace Musicx.Application.Shared.Helpers;

/// <summary>
/// List of extension methods for Enums.
/// </summary>
/// <since>0.6.1</since>
public static class EnumHelper
{
    /// <summary>
    /// Retrieve an enum or a default value if not found.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    /// <since>0.6.1</since>
    public static TEnum ParseOrDefault<TEnum>(string value, TEnum defaultValue) where TEnum : struct, Enum
    {
        return Enum.TryParse<TEnum>(value, out var result) ? result : defaultValue;
    } 
}