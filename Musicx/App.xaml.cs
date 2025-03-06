using System.Configuration;
using System.Data;
using System.Windows;
using log4net;
using Microsoft.VisualBasic.Logging;
using Musicx.Database;

namespace Musicx;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static AppDbContext DbContext { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DbContext = new AppDbContext();
        DbContext.Database.EnsureCreated();
    }
}