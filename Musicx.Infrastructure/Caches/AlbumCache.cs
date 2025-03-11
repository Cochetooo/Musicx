using Musicx.Core.Models;

namespace Musicx.Infrastructure.Caches;

public interface IAlbumCache
{
    Album? Get(string key);
    void Add(string key, Album album);
}

public class AlbumCache : IAlbumCache
{
    private readonly Dictionary<string, Album> _cache = new();
    private const int MaxCacheSize = 5000;
    
    public Album? Get(string key) => _cache.TryGetValue(key, out var album) ? album : null;

    public void Add(string key, Album album)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = album;
    }
}