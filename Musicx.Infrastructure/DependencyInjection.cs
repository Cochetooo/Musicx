using Microsoft.Extensions.DependencyInjection;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Logging;
using Musicx.Infrastructure.Managers;
using Musicx.Infrastructure.Repositories;

namespace Musicx.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerFactory, Log4NetLoggerFactory>();
        
        // 🔹 Ajout de DbContext
        services.AddDbContext<AppDbContext>();
        
        // 🔹 Ajout des repositories
        services.AddScoped<IRepository<ArtistEntity>, ArtistRepository>();
        services.AddScoped<IRepository<AlbumEntity>, AlbumRepository>();
        services.AddScoped<IRepository<SongEntity>, SongRepository>();
        services.AddScoped<IRepository<LabelEntity>, LabelRepository>();
        services.AddScoped<IRepository<GenreEntity>, GenreRepository>();
        
        // 🔹 Ajout des managers
        services.AddScoped<IManager<Artist>, ArtistManager>();
        services.AddScoped<IManager<Album>, AlbumManager>();
        services.AddScoped<IManager<Song>, SongManager>();
        services.AddScoped<IManager<Label>, LabelManager>();
        services.AddScoped<IManager<Genre>, GenreManager>();

        return services;
    }
}