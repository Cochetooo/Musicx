using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Infrastructure.API.Persistence;
using Musicx.Infrastructure.Desktop.Persistence;
using Musicx.Infrastructure.Desktop.Persistence.Caches;
using Musicx.Infrastructure.Desktop.Persistence.Repositories;
using Musicx.Infrastructure.Desktop.Services.LocalLibrary;
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
        services.AddDbContext<DbContext, AppDbContext>();
        
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
    /// Add Api module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<DbContext, ApiDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        // Repositories
        services.AddScoped<Application.Api.Interfaces.Persistence.ISongRepository, API.Persistence.Repositories.SongRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IAlbumRepository, API.Persistence.Repositories.AlbumRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IArtistRepository, API.Persistence.Repositories.ArtistRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IReleaseRepository, API.Persistence.Repositories.ReleaseRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.ILabelRepository, API.Persistence.Repositories.LabelRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IGenreRepository, API.Persistence.Repositories.GenreRepository>();
        
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