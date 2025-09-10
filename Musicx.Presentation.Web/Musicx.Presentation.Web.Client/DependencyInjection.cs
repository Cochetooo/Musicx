using MudBlazor.Services;
using Musicx.Application.Web.Interfaces.Models.Auth;
using Musicx.Presentation.Web.Client.Models.Auth;

namespace Musicx.Presentation.Web.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddFrontFramework(this IServiceCollection services)
    {
        services
            .AddMudServices();

        services.AddScoped<IUserClientContext, UserClientContext>();
        
        return services;
    } 
}