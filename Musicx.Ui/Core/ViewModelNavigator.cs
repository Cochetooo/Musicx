using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Ui.Pages.LocalLibrary.ViewModels;
using Musicx.Ui.Pages.PageSelector.ViewModels;

namespace Musicx.Ui.Core;

public interface IViewModelNavigator
{
    object CurrentViewModel { get; }
    void ChangeViewModel(object viewModel);
}

public partial class ViewModelNavigator : ObservableObject, IViewModelNavigator
{
    [ObservableProperty] 
    private object _currentViewModel;

    public void ChangeViewModel(object viewModel)
    {
        CurrentViewModel = viewModel;
    }
}
