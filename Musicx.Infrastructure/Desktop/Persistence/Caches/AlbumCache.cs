using Microsoft.Extensions.Configuration;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class AlbumCache : IAlbumCache
{
    private const int MaxCacheSize = 1000;
    private readonly Dictionary<string, OutAlbum> _cache = new();

    public OutAlbum? Get(string key) => _cache.TryGetValue(key, out var album) ? album : null;

    public void Add(string key, OutAlbum album)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = album;
    }
}