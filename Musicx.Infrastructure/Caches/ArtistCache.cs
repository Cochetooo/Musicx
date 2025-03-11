using Musicx.Core.Models;

namespace Musicx.Infrastructure.Caches;

public interface IArtistCache
{
    Artist? Get(string key);
    void Add(string key, Artist artist);
}

public class ArtistCache : IArtistCache
{
    private readonly Dictionary<string, Artist> _cache = new();
    private const int MaxCacheSize = 1000;
    
    public Artist? Get(string key) => _cache.TryGetValue(key, out var artist) ? artist : null;

    public void Add(string key, Artist artist)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = artist;
    }
}