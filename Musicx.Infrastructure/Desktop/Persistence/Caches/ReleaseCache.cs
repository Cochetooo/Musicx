using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class ReleaseCache : IReleaseCache
{
    private const int MaxCacheSize = 1000;
    private readonly Dictionary<string, OutRelease> _cache = new();

    public OutRelease? Get(string key) => _cache.TryGetValue(key, out var release) ? release : null;

    public void Add(string key, OutRelease release)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = release;
    }
}