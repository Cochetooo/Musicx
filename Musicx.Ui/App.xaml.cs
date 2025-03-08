using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Data;
using Musicx.Infrastructure;
using Musicx.Ui.Services;
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

    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
        {
            Console.WriteLine($"[EXCEPTION] {eventArgs.Exception}");
        };

        
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
        
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // 🔹 Ajout de l'infrastructure
        services.AddInfrastructure();
        
        // 🔹 Ajout des services UI
        services.AddSingleton<NavigationService>();
        
        // 🔹 Ajout des view models
        services.AddTransient<ModuleSelectorViewModel>();
        services.AddTransient<SongListViewModel>();

        services.AddTransient<MainViewModel>();
        
        // 🔹 Ajout des views
        services.AddTransient<ModuleSelector>();
        services.AddTransient<SongList>();
        
        services.AddTransient<MainWindow>();
    }
}