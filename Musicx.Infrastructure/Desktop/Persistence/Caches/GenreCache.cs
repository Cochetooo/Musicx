using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;


namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class GenreCache : IGenreCache
{
    private const int MaxCacheSize = 5000;
    private readonly Dictionary<string, OutGenre> _cache = new();

    public OutGenre? Get(string key) => _cache.TryGetValue(key, out var genre) ? genre : null;

    public void Add(string key, OutGenre genre)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = genre;
    }
}