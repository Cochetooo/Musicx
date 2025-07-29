namespace Musicx.Contracts.Helpers;

public static class DictionaryHelper
{
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