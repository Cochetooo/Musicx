using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class LabelCache : ILabelCache
{
    private const int MaxCacheSize = 1000;
    private readonly Dictionary<string, OutLabel> _cache = new();

    public OutLabel? Get(string key) => _cache.TryGetValue(key, out var label) ? label : null;

    public void Add(string key, OutLabel label)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = label;
    }
}