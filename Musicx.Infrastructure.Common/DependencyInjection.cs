using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Common.Interfaces.Common;
using Musicx.Infrastructure.Common.Logging;

namespace Musicx.Infrastructure.Common;

public static class DependencyInjection
{
    /// <summary>
    /// Add common module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxInfrastructure(this IServiceCollection services)
    {
        // Logger
        services.AddSingleton<ILoggerFactory, Log4NetLoggerFactory>();
        
        return services;
    }
}