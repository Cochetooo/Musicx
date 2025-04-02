using Musicx.Application.Common.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Domain.Entities;

namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

public class ReleaseCache(IAppConfiguration appConfiguration) : IReleaseCache
{
    private readonly int _maxCacheSize = appConfiguration.GetValue<int>("Persistence.CacheMaxSizes.Release");
    private readonly Dictionary<string, Release> _cache = new();

    public Release? Get(string key) => _cache.TryGetValue(key, out var release) ? release : null;

    public void Add(string key, Release release)
    {
        if (_cache.Count >= _maxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = release;
    }
}