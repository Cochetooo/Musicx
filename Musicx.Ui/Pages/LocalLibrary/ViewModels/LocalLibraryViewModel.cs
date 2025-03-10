using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Infrastructure.Managers;
using Musicx.Ui.Core;
using NAudio.Wave;
using Wpf.Ui.Input;

namespace Musicx.Ui.Pages.LocalLibrary.ViewModels;

public partial class LocalLibraryViewModel : ObservableObject
{
    private readonly ILogger Logger;
    private readonly IManager<Song> _songManager;

    [ObservableProperty]
    private ObservableCollection<Song> _songs = [];
    
    [ObservableProperty]
    private Song? _selectedSong;
    
    public ICommand LoadedCommand { get; }

    public LocalLibraryViewModel(ILoggerFactory loggerFactory, ISongManager songManager)
    {
        Logger = loggerFactory.CreateLogger(typeof(LocalLibraryViewModel));
        _songManager = songManager;
        
        LoadedCommand = new RelayCommand(async() => await OnComponentLoaded());
    }

    private async Task OnComponentLoaded()
    {
        await LoadSongs();
    }

    private async Task LoadSongs()
    {
        Logger.Info("⛏️ Loading songs...");
        var listSongs = await _songManager.FindAll(take: 200);
        foreach (var song in listSongs)
        {
            Songs.Add(song);
        }
        Logger.Info("✅ Songs loaded successfully!");
    }
}