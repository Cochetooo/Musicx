namespace Musicx.Application.Shared.Helpers;

/// <summary>
/// List of extension methods for Dictionary&lt;T&gt;.
/// </summary>
/// <since>0.6.1</since>
public static class DictionaryHelper
{
    /// <summary>
    /// Retrieve a value from a key or a default value if key does not exist.
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <since>0.6.1</since>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default!)
    {
        return dict.TryGetValue(key, out var value) ? value : defaultValue;
    }
    
    public static T? SafeGet<T>(this IDictionary<string, object?> dict, string key)
    {
        if (!dict.TryGetValue(key, out var value) || value is null)
        {
            return default;
        }

        try
        {
            var targetType = typeof(T);
            var actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            
            if (value is T t)
            {
                return t;
            }

            if (actualType.IsEnum)
            {
                if (value is string s)
                {
                    if (Enum.TryParse(actualType, s, ignoreCase: true, out var parsedEnum))
                    {
                        return (T?)parsedEnum;
                    }
                }
                else
                {
                    var numericValue = Convert.ToInt32(value);                    
                    return (T?)Enum.ToObject(actualType, numericValue);
                }
            }
            
            return (T?) Convert.ChangeType(value, actualType);
        }
        catch
        {
            return default;
        }
    }
}