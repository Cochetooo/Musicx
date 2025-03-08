using Musicx.Core.Logging;
using Musicx.Ui.Views.Content;

namespace Musicx.Ui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow(ModuleSelector moduleSelector)
    {
        InitializeComponent();
        
        ContentHost.Content = moduleSelector;
    }
}