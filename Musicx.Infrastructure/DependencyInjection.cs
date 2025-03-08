using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Logging;
using Musicx.Infrastructure.Managers;
using Musicx.Infrastructure.Repositories;
using Musicx.Infrastructure.Services;

namespace Musicx.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerFactory, Log4NetLoggerFactory>();
        
        // 🔹 Ajout de DbContext
        services.AddDbContext<AppDbContext>();
        
        // 🔹 Ajout des services
        services.AddServices(Assembly.Load("Musicx.Infrastructure"));
        
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

    public static IServiceCollection AddServices(this IServiceCollection services, Assembly assembly)
    {
        var serviceType = assembly.GetTypes()
            .Where(t => typeof(IService).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });
        
        foreach (var service in serviceType)
        {
            var interfaceType = service.GetInterfaces().FirstOrDefault(i => i != typeof(IService));
            if (null != interfaceType)
            {
                services.AddSingleton(interfaceType, service);
            }
        }
        
        return services;
    }
}