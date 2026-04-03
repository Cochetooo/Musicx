using System;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Musicx.Infrastructure;
using Musicx.Presentation.Desktop.Providers;
using Musicx.Presentation.Desktop.ViewModels;
using Musicx.Presentation.Desktop.ViewModels.Pages;
using Musicx.Presentation.Desktop.Views;

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
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        DataTemplates.Add(Services.GetRequiredService<ViewLocator>());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            
            var vm = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow { DataContext = vm };
            
            var libraryVm = Services.GetRequiredService<LibraryPageViewModel>();
            _ = libraryVm.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddLog4Net("log4net.config");
            builder.AddProvider(new StyledConsoleLoggerProvider());
        });
        
        services
            .AddMusicxLocalization()
            .AddMusicxDesktop();
        services.AddSingleton<ViewLocator>();
        
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<BottomAudioPlayerViewModel>();
        services.AddSingleton<ArtistPageViewModel>();
        services.AddSingleton<AlbumPageViewModel>();
        services.AddSingleton<GenrePageViewModel>();
        services.AddSingleton<UserPageViewModel>();
        services.AddSingleton<LibraryPageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var toRemove = BindingPlugins
            .DataValidators
            .OfType<DataAnnotationsValidationPlugin>()
            .ToArray();
        
        foreach (var plugin in toRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}