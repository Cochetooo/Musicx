using MudBlazor.Services;

namespace Musicx.Presentation.Web.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddFrontFramework(this IServiceCollection services)
    {
        services
            .AddMudServices();
        
        return services;
    } 
}