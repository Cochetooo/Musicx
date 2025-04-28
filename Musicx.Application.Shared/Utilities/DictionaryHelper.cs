namespace Musicx.Application.Shared.Utilities;

public static class DictionaryHelper
{
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default!)
    {
        return dict.TryGetValue(key, out var value) ? value : defaultValue;
    }
}