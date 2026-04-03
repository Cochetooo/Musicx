using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Desktop.UseCases.Library;

namespace Musicx.Application.Desktop;

public static class DependencyInjection
{
    /// <summary>
    /// Add application module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxDesktopApp(this IServiceCollection services)
    {
        // Non external framework dependant use cases
        services.AddScoped<IGetOrCreateDefaultProfileUseCase, GetOrCreateDefaultProfileUseCase>();
        services.AddScoped<IUpsertLibraryProfileUseCase, UpsertLibraryProfileUseCase>();
        services.AddScoped<IImportLibraryUseCase, ImportLibraryUseCase>();
        services.AddScoped<IGetLibrarySnapshotUseCase, GetLibrarySnapshotUseCase>();
        
        return services;
    }
}