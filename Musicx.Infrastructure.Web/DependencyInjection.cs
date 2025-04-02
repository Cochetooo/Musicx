using Microsoft.Extensions.DependencyInjection;

namespace Musicx.Infrastructure.Web;

public static class DependencyInjection
{
    /// <summary>
    /// Add web module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxWeb(this IServiceCollection services)
    {
        return services;
    }
}