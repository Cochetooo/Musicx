using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Infrastructure.Persistence;
using Musicx.Infrastructure.Persistence.Caches;
using Musicx.Infrastructure.Persistence.Repositories;
using Musicx.Infrastructure.Services.LocalLibrary;
using Musicx.Infrastructure.Shared.Logging;

namespace Musicx.Infrastructure;

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
    
    /// <summary>
    /// Add desktop module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxDesktop(this IServiceCollection services)
    {
        // Database
        services.AddDbContext<AppDbContext>();
        
        // Caches
        services.AddScoped<ISongCache, SongCache>();
        services.AddScoped<IAlbumCache, AlbumCache>();
        services.AddScoped<IArtistCache, ArtistCache>();
        services.AddScoped<IReleaseCache, ReleaseCache>();
        services.AddScoped<ILabelCache, LabelCache>();
        services.AddScoped<IGenreCache, GenreCache>();
        
        // Repositories
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IReleaseRepository, ReleaseRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        
        services.AddScoped<IBatchImportRepository, BatchImportRepository>();
        
        // Use cases
        services.AddScoped<IReadAudioFileUseCase, UcReadAudioFile>();
        
        return services;
    }
    
    /// <summary>
    /// Add web module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxWeb(this IServiceCollection services)
    {
        return services;
    }
}