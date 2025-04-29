using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Persistence.Caches;

internal sealed class LabelCache(IAppConfiguration appConfiguration) : ILabelCache
{
    private readonly int _maxCacheSize = appConfiguration.GetValue<int>("Persistence.CacheMaxSizes.Label");
    private readonly Dictionary<string, Label> _cache = new();

    public Label? Get(string key) => _cache.TryGetValue(key, out var label) ? label : null;

    public void Add(string key, Label label)
    {
        if (_cache.Count >= _maxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = label;
    }
}