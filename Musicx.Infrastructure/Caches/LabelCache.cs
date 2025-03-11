using Musicx.Core.Models;

namespace Musicx.Infrastructure.Caches;

public interface ILabelCache
{
    Label? Get(string key);
    void Add(string key, Label label);
}

public class LabelCache : ILabelCache
{
    private readonly Dictionary<string, Label> _cache = new();
    private const int MaxCacheSize = 1000;
    
    public Label? Get(string key) => _cache.TryGetValue(key, out var label) ? label : null;

    public void Add(string key, Label label)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = label;
    }
}