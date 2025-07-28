namespace Musicx.Contracts.Helpers;

public static class DictionaryHelper
{
    public static T? SafeGet<T>(this IDictionary<string, object?> dict, string key)
    {
        if (dict.TryGetValue(key, out var value) && value is T t)
        {
            return t;
        }

        try
        {
            return (T?) Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }
}