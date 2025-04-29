using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Persistence.Caches;

internal sealed class ArtistCache(IAppConfiguration appConfiguration) : IArtistCache
{
    private readonly int _maxCacheSize = appConfiguration.GetValue<int>("Persistence.CacheMaxSizes.Artist");
    private readonly Dictionary<string, Artist> _cache = new();

    public Artist? Get(string key) => _cache.TryGetValue(key, out var artist) ? artist : null;

    public void Add(string key, Artist artist)
    {
        if (_cache.Count >= _maxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = artist;
    }
}