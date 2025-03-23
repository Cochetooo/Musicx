using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Core.Logging;
using Musicx.Data;
using Musicx.Infrastructure;
using Musicx.Infrastructure.Listeners;
using Musicx.Ui.Core;
using Musicx.Ui.Main.ViewModels;
using Musicx.Ui.Main.Views;
using Musicx.Ui.Pages.LocalLibrary.ViewModels;
using Musicx.Ui.Pages.LocalLibrary.Views;
using Musicx.Ui.Pages.PageSelector.ViewModels;
using Musicx.Ui.Pages.PageSelector.Views;
using Musicx.Ui.UIComponents.AppInfoBar.ViewModels;
using Musicx.Ui.UIComponents.AudioPlayer.Services;
using Musicx.Ui.UIComponents.AudioPlayer.ViewModels;
using ViewModelNavigator = Musicx.Ui.Core.ViewModelNavigator;

namespace Musicx.Ui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static ILogger<App> Logger;
    public static IServiceProvider ServiceProvider { get; set; }

    public static AppDbContext DbContext { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        
        ConfigureServices(services);
        
        ServiceProvider = services.BuildServiceProvider();
        
        ViewModelLocator.ServiceProvider = ServiceProvider;
        
        // 🔹 Assurer que la base de données est créée
        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated(); // 📌 Vérifie et crée la base si elle n'existe pas
        }
        
        base.OnStartup(e);
        
        Logger.Info("⛏️ Creating Main Window...");
        var mainWindow = ServiceProvider.GetRequiredService<MainWindowView>();
        
        Logger.Info("🟢 Initialization complete, running program!");
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        
        Logger.Info("🔴 Program exiting.");
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // 🔹 Ajout de l'infrastructure
        services.AddInfrastructure();
        
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            Logger = loggerFactory.CreateLogger<App>();
        
            AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
            {
                Logger.Fatal("❌ An exception from WPF has been thrown.", eventArgs.Exception);
            };
        }
        
        // 🔹 Ajout des services UI
        services.AddSingleton<IProgressListener, ProgressListener>();
        services.AddSingleton<AudioPlayerService>();
        services.AddSingleton<AudioQueueService>();
        
        // 🔹 Ajout des view models
        Logger.Info("⛏️ Loading View Models...");
        services.AddSingleton<IViewModelNavigator, ViewModelNavigator>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<AppInfoViewModel>();
        services.AddSingleton<AudioPlayerViewModel>();
        services.AddSingleton<LocalLibraryViewModel>();
        
        // Factory-based injection
        services.AddPageFactory<PageSelectorViewModel>();
        
        // 🔹 Ajout des views
        Logger.Info("⛏️ Loading Views...");
        services.AddSingleton<MainWindowView>();
    }
}