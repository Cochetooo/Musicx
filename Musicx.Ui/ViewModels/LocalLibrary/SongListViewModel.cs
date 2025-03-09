using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Accessibility;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Infrastructure.Managers;
using Musicx.Ui.Commands;

namespace Musicx.Ui.ViewModels.LocalLibrary;

public class SongListViewModel
{
    private readonly ILogger Logger;

    private readonly IManager<Song> _songManager;

    public ObservableCollection<Song> Songs { get; set; } = [];
    
    public ICommand LoadedCommand { get; }

    public SongListViewModel(ILoggerFactory loggerFactory, ISongManager songManager)
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
        Logger.Info("⛏️ Loading songs...");
        var listSongs = await _songManager.FindAll();
        foreach (var song in listSongs)
        {
            Songs.Add(song);
        }
        Logger.Info("✅ Songs loaded successfully!");
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}