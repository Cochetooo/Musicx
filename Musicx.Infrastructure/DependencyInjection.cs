using System.Reflection;
using System.Text;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Caches;
using Musicx.Infrastructure.Logging;
using Musicx.Infrastructure.Managers;
using Musicx.Infrastructure.Repositories;
using Musicx.Infrastructure.Services;
using Musicx.Infrastructure.Services.LocalLibrary;

namespace Musicx.Infrastructure;

public static class DependencyInjection
{
    private static ILog Logger = LogManager.GetLogger(typeof(DependencyInjection));
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerFactory, Log4NetLoggerFactory>();
        
        Logger.Info("⛏️ Injecting Dependencies from Infrastructure module...");
        
        // 🔹 Ajout de DbContext
        services.AddDbContext<AppDbContext>();
        
        Logger.Info("⛏️ Loading Infrastructure Services...");
        // 🔹 Ajout des services
        services.AddServices(Assembly.Load("Musicx.Infrastructure"));
        
        // 🔹 Ajout des repositories
        Logger.Info("⛏️ Loading Infrastructure Repositories...");
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        
        // 🔹 Ajout des caches
        Logger.Info("⛏️ Loading Infrastructure Caches...");
        services.AddSingleton<ISongCache, SongCache>();
        services.AddSingleton<IAlbumCache, AlbumCache>();
        services.AddSingleton<IArtistCache, ArtistCache>();
        services.AddSingleton<ILabelCache, LabelCache>();
        services.AddSingleton<IGenreCache, GenreCache>();
        
        // 🔹 Ajout des managers
        Logger.Info("⛏️ Loading Infrastructure Managers...");
        services.AddScoped<IArtistManager, ArtistManager>();
        services.AddScoped<IAlbumManager, AlbumManager>();
        services.AddScoped<ISongManager, SongManager>();
        services.AddScoped<ILabelManager, LabelManager>();
        services.AddScoped<IGenreManager, GenreManager>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, Assembly assembly)
    {
        var serviceType = assembly.GetTypes()
            .Where(t => typeof(IService).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });
        
        foreach (var service in serviceType)
        {
            Logger.Debug($"🔍 Loading Service: {service.FullName}");
            services.AddScoped(service);
        }
        
        return services;
    }
}