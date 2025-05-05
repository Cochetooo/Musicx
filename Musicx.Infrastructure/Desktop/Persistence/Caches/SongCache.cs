using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class SongCache : ISongCache
{
    private const int MaxCacheSize = 10_000;
    private readonly Dictionary<string, Song> _cache = new();

    public Song? Get(string key) => _cache.TryGetValue(key, out var song) ? song : null;

    public void Add(string key, Song song)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = song;
    }
}