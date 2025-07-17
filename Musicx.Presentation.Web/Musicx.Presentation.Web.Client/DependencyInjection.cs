using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;

namespace Musicx.Presentation.Web.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddFrontFramework(this IServiceCollection services)
    {
        services
            .AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddMaterialProviders()
            .AddMaterialIcons();
        
        return services;
    } 
}