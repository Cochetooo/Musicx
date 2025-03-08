using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Accessibility;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Infrastructure.Managers;
using Musicx.Ui.Commands;

namespace Musicx.Ui.ViewModels.LocalLibrary;

public class SongListViewModel
{
    private readonly ILogger Logger;

    private SongManager _songManager;

    public ObservableCollection<Song> Songs { get; set; } = [];
    
    public ICommand LoadedCommand { get; }

    public SongListViewModel(ILoggerFactory loggerFactory, SongManager songManager)
    {
        LoadedCommand = new RelayCommand(async() => await OnComponentLoaded());
        Logger = loggerFactory.CreateLogger(typeof(SongListViewModel));
        _songManager = songManager;
    }

    private async Task OnComponentLoaded()
    {
        await LoadSongs();
    }

    private async Task LoadSongs()
    {
        var listSongs = await _songManager.FindAll();
        foreach (var song in listSongs)
        {
            Songs.Add(song);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}