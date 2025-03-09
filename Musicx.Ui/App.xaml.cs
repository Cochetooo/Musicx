using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Core.Logging;
using Musicx.Data;
using Musicx.Infrastructure;
using Musicx.Ui.ViewModels;
using Musicx.Ui.ViewModels.Content;
using Musicx.Ui.ViewModels.LocalLibrary;
using Musicx.Ui.Views.Content;
using Musicx.Ui.Views.LocalLibrary;

namespace Musicx.Ui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static IServiceProvider ServiceProvider { get; set; }
    
    public static AppDbContext DbContext { get; private set; }

    private static ILogger Logger;

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
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        
        Logger.Info("🟢 Initialization complete, running program!");
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // 🔹 Ajout de l'infrastructure
        services.AddInfrastructure();

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            Logger = loggerFactory.CreateLogger(typeof(DependencyInjection));
        
            AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
            {
                Logger.Fatal("❌ An exception from WPF has been thrown.", eventArgs.Exception);
            };
        }
        
        // 🔹 Ajout des services UI
        
        // 🔹 Ajout des view models
        Logger.Info("⛏️ Loading View Models...");
        services.AddTransient<Func<ModuleSelectorViewModel>>(provider => provider.GetRequiredService<ModuleSelectorViewModel>);
        services.AddTransient<Func<SongListViewModel>>(provider => provider.GetRequiredService<SongListViewModel>);
        
        services.AddSingleton<IViewModelNavigator, ViewModelNavigator>();
        services.AddSingleton<MainWindowViewModel>();
        
        // Factory-based injection
        services.AddTransient<ModuleSelectorViewModel>();
        services.AddTransient<SongListViewModel>();

        // Enregistre Func<T> pour permettre la création différée des ViewModels
        services.AddSingleton<Func<ModuleSelectorViewModel>>(sp => sp.GetRequiredService<ModuleSelectorViewModel>);
        services.AddSingleton<Func<SongListViewModel>>(sp => sp.GetRequiredService<SongListViewModel>);

        
        // 🔹 Ajout des views
        Logger.Info("⛏️ Loading Views...");
        services.AddTransient<ModuleSelectorView>();
        services.AddTransient<SongListView>();
        
        services.AddSingleton<MainWindow>();
    }
}