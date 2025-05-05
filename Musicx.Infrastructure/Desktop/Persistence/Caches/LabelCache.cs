using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class LabelCache : ILabelCache
{
    private const int MaxCacheSize = 1000;
    private readonly Dictionary<string, Label> _cache = new();

    public Label? Get(string key) => _cache.TryGetValue(key, out var label) ? label : null;

    public void Add(string key, Label label)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = label;
    }
}