using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class ArtistCache : IArtistCache
{
    private const int MaxCacheSize = 500;
    private readonly Dictionary<string, OutArtist> _cache = new();

    public OutArtist? Get(string key) => _cache.TryGetValue(key, out var artist) ? artist : null;

    public void Add(string key, OutArtist artist)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = artist;
    }
}