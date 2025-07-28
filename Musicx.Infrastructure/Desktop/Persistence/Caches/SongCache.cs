using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class SongCache : ISongCache
{
    private const int MaxCacheSize = 10_000;
    private readonly Dictionary<string, OutSong> _cache = new();

    public OutSong? Get(string key) => _cache.TryGetValue(key, out var song) ? song : null;

    public void Add(string key, OutSong song)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = song;
    }
}