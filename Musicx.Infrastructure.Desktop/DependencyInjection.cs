using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Infrastructure.Desktop.Persistence;
using Musicx.Infrastructure.Desktop.Persistence.Caches;
using Musicx.Infrastructure.Desktop.Persistence.Repositories;
using Musicx.Infrastructure.Desktop.Services.LocalLibrary;

namespace Musicx.Infrastructure.Desktop;

public static class DependencyInjection
{
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
        
        // Use cases
        services.AddScoped<IReadAudioFileUseCase, UcReadAudioFile>();
        
        return services;
    }
}