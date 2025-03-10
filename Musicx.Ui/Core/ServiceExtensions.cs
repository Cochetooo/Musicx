using Microsoft.Extensions.DependencyInjection;

namespace Musicx.Ui.Core;

public static class ServiceExtensions
{
    public static void AddPageFactory<TPage>(this IServiceCollection services)
        where TPage : class
    {
        services.AddTransient<TPage>();
        services.AddSingleton<Func<TPage>>(sp => sp.GetRequiredService<TPage>);
        services.AddSingleton<IAbstractFactory<TPage>, AbstractFactory<TPage>>();
    }
}