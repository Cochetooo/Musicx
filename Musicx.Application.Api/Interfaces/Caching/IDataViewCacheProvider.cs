namespace Musicx.Application.Api.Interfaces.Caching;

public interface IDataViewCacheProvider
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class;
    
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        where T : class;
}