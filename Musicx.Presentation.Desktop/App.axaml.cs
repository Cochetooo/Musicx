using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Infrastructure;
using Musicx.Presentation.Desktop.Services;
using Musicx.Presentation.Desktop.ViewModels;
using Musicx.Presentation.Desktop.ViewModels.Body;
using Musicx.Presentation.Desktop.ViewModels.Body.Content;
using Musicx.Presentation.Desktop.Views;
using Musicx.Presentation.Desktop.Views.Body;
using Musicx.Presentation.Desktop.Views.Body.Content;

namespace Musicx.Presentation.Desktop;

public partial class App : Avalonia.Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        
        ConfigureServices(serviceCollection);
        
        Services = serviceCollection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            
            var mainWindow = Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services
            .AddMusicxInfrastructure()
            .AddMusicxDesktop();

        services.AddSingleton<ViewLocator>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();
        
        services.AddSingleton<MainTitleBarViewModel>();
        services.AddSingleton<SidebarLeftViewModel>();
        services.AddSingleton<SidebarRightViewModel>();
        services.AddSingleton<BottomAudioPlayerViewModel>();

        services.AddSingleton<LibraryManageView>();
        services.AddSingleton<LibraryManageViewModel>();
        
        services.AddSingleton<Func<Window>>(() => ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow!
            : throw new InvalidOperationException("No main window"));
        
        services.AddSingleton<IWindowProvider, WindowProvider>();
    }
}