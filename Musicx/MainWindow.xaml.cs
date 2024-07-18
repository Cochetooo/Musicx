using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using log4net;
using Wpf.Ui.Controls;

namespace Musicx;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindow));
    
    public MainWindow()
    {
        Log.Info("Initializing program...");
        var stopwatch = new Stopwatch();
        
        stopwatch.Start();
        
        InitializeComponent();
        
        stopwatch.Stop();
        Log.Info("Program initialized in " + stopwatch.ElapsedMilliseconds + "ms.");
    }
}