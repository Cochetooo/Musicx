using Musicx.Application.Api.Interfaces.Caching;

namespace Musicx.Infrastructure.API.Caching;

public sealed class NoOpDataViewCacheProvider : IDataViewCacheProvider
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        => Task.FromResult<T?>(null);

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default) where T : class
        => Task.CompletedTask;
}