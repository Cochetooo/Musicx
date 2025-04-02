using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Desktop.UseCases.LocalLibrary;

namespace Musicx.Application.Desktop;

public static class DependencyInjection
{
    /// <summary>
    /// Add application module services and use cases.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Non external framework dependant use cases
        services.AddScoped<IImportLocalSongsUseCase, UcImportLocalSongs>();
        services.AddScoped<IPersistLocalSongsUseCase, UcPersistLocalSongs>();
        
        return services;
    }
}