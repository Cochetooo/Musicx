using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Persistence.Caches;

internal sealed class AlbumCache(IAppConfiguration appConfiguration) : IAlbumCache
{
    private readonly int _maxCacheSize = appConfiguration.GetValue<int>("Persistence.CacheMaxSizes.Album");
    private readonly Dictionary<string, Album> _cache = new();

    public Album? Get(string key) => _cache.TryGetValue(key, out var album) ? album : null;

    public void Add(string key, Album album)
    {
        if (_cache.Count >= _maxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = album;
    }
}