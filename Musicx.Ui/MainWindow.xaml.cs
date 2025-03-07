using log4net;

namespace Musicx.Ui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(MainWindow));
    
    public MainWindow()
    {
        InitializeComponent();
    }
}