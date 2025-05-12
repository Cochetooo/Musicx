namespace Musicx.Application.Shared.Utilities;

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
}