using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Ui.Core;
using Musicx.Ui.Pages.PageSelector.ViewModels;

namespace Musicx.Ui.Main.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] 
    private IViewModelNavigator _navigator;

    public MainWindowViewModel(IViewModelNavigator navigator, IAbstractFactory<PageSelectorViewModel> pageSelectorFactory)
    {
        Navigator = navigator;
        Navigator.ChangeViewModel(pageSelectorFactory.Create());
    }
}