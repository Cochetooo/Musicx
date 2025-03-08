using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Listeners;
using Musicx.Infrastructure.Services.LocalLibrary;
using Musicx.Ui.Commands;
using Wpf.Ui.Input;

namespace Musicx.Ui.ViewModels.Content;

public class ModuleSelectorViewModel
{
    private readonly ILogger Logger;
    
    public ICommand BrowseLocalLibraryCommand { get; }
    private ImportLocalSongsService ImportLocalSongsService { get; }

    public ModuleSelectorViewModel(
        ILoggerFactory loggerFactory, 
        ImportLocalSongsService importLocalSongsService)
    {
        Logger = loggerFactory.CreateLogger(typeof(ModuleSelectorViewModel));
        BrowseLocalLibraryCommand = new RelayCommand(async () => await BrowseLocalLibrary());
        ImportLocalSongsService = importLocalSongsService;
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