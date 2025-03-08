using System.Windows.Input;
using Musicx.Ui.Commands;
using Musicx.Ui.Services;
using Musicx.Ui.ViewModels.Content;
using Musicx.Ui.ViewModels.LocalLibrary;

namespace Musicx.Ui.ViewModels;

public class MainViewModel
{
    public NavigationService Navigation { get; }
    
    public ICommand ShowModuleSelectorCommand { get; }
    public ICommand ShowSongListCommand { get; }

    public MainViewModel(NavigationService navigationService,
        ModuleSelectorViewModel moduleSelectorVm,
        SongListViewModel songListVm)
    {
        Navigation = navigationService;
        
        Navigation.NavigateTo(moduleSelectorVm);
        
        ShowModuleSelectorCommand = new RelayCommand(() =>
        {
            Navigation.NavigateTo(moduleSelectorVm);
            return Task.CompletedTask;
        });
        
        ShowSongListCommand = new RelayCommand(() =>
        {
            Navigation.NavigateTo(songListVm);
            return Task.CompletedTask;
        });
    }
}