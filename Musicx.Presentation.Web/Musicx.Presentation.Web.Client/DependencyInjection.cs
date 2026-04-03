using MudBlazor;
using MudBlazor.Services;
using Musicx.Application.Web.Interfaces.Models.Auth;
using Musicx.Application.Web.ViewModels.User.GenreRatings;
using Musicx.Infrastructure;
using Musicx.Presentation.Web.Client.Models.Auth;

namespace Musicx.Presentation.Web.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddFrontFramework(this IServiceCollection services)
    {
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.VisibleStateDuration = 6000;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        services.AddScoped<IUserClientContext, UserClientContext>();
        services.AddScoped<UserGenreRatingsViewModel>();
        services.AddMusicxLocalization(ServiceLifetime.Scoped);
        
        return services;
    } 
}