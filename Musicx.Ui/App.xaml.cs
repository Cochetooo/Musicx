using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Data;
using Musicx.Infrastructure;

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
        var services = new ServiceCollection();
        
        ConfigureServices(services);
        
        ServiceProvider = services.BuildServiceProvider();
        
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
        
        // 🔹 Ajout de la MainWindow
        services.AddTransient<MainWindow>();
    }
}