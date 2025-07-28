namespace Musicx.Contracts.Helpers;

public static class DictionaryHelper
{
    public static T? Get<T>(this IDictionary<string, object?> dict, string key) where T : struct
    {
        return dict.TryGetValue(key, out var value) && value is T typed ? typed : default;
    }
}