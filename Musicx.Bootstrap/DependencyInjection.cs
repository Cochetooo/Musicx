using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Interfaces.Common;
using Musicx.Application.Interfaces.LocalLibrary;
using Musicx.Application.Interfaces.Persistence;
using Musicx.Application.UseCases.LocalLibrary;
using Musicx.Infrastructure.Desktop.Logging;
using Musicx.Infrastructure.Desktop.Persistence;
using Musicx.Infrastructure.Desktop.Persistence.Caches;
using Musicx.Infrastructure.Desktop.Persistence.Repositories;
using Musicx.Infrastructure.Desktop.Services.LocalLibrary;

namespace Musicx.Bootstrap;

/// <summary>
/// Centralize services configurations from every module.
/// </summary>
/// <since>0.6.1</since>
public static class DependencyInjection
{
    

    /// <summary>
    /// Add infrastructure module services and use cases.
    /// </summary>
    public static IServiceCollection AddInfrastructureDesktop(this IServiceCollection services)
    {
        
        
        return services;
    }
}