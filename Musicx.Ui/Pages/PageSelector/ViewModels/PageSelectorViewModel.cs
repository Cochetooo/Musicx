using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Listeners;
using Musicx.Infrastructure.Managers;
using Musicx.Infrastructure.Services.LocalLibrary;
using Musicx.Ui.Core;
using Musicx.Ui.Pages.LocalLibrary.ViewModels;

namespace Musicx.Ui.Pages.PageSelector.ViewModels;

public class PageSelectorViewModel
{
    private readonly ILogger Logger;
    
    public ICommand BrowseLocalLibraryCommand { get; }
    private ImportLocalSongsService ImportLocalSongsService { get; }

    private readonly IViewModelNavigator ViewModelNavigator;
    private LocalLibraryViewModel _localLibraryVm;

    public PageSelectorViewModel(
        ILoggerFactory loggerFactory, 
        ISongManager songManager,
        ImportLocalSongsService importLocalSongsService,
        IViewModelNavigator viewModelNavigator,
        LocalLibraryViewModel localLibraryVm)
    {
        var existingSong = songManager.FindAll();
        if (existingSong.Result.Any())
        {
            viewModelNavigator.ChangeViewModel(localLibraryVm);
        }
        
        Logger = loggerFactory.CreateLogger(typeof(PageSelectorViewModel));
        BrowseLocalLibraryCommand = new RelayCommand(async () => await BrowseLocalLibrary());
        ImportLocalSongsService = importLocalSongsService;
        ViewModelNavigator = viewModelNavigator;
        _localLibraryVm = localLibraryVm;
    }

    private async Task BrowseLocalLibrary()
    {
        var fileBrowse = new OpenFolderDialog()
        {
            Title = "Select audio folder",
            Multiselect = true,
        };
        
        if (true == fileBrowse.ShowDialog())
        {
            await ImportLocalSongsService.Execute(fileBrowse.FolderNames.ToList(), [".mp3"], new ProgressListener());
            ViewModelNavigator.ChangeViewModel(_localLibraryVm);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    class ProgressListener : IProgressListener
    {
        public void UpdateProgress(int progress, int total)
        {

        }
    }
}