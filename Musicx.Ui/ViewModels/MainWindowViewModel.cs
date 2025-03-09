using Musicx.Ui.ViewModels.Content;
using Musicx.Ui.ViewModels.LocalLibrary;

namespace Musicx.Ui.ViewModels;

public class MainWindowViewModel
{
    public IViewModelNavigator Navigator { get; }

    public MainWindowViewModel(IViewModelNavigator navigator, Func<ModuleSelectorViewModel> moduleSelectorVmFactory)
    {
        Navigator = navigator;
        Navigator.ChangeViewModel(moduleSelectorVmFactory());
    }
}