using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Infrastructure.Loaders;
using Musicx.Infrastructure.Managers;
using Musicx.Ui.Core;
using Musicx.Ui.UIComponents.AudioPlayer.ViewModels;
using NAudio.Wave;
using Wpf.Ui.Input;

namespace Musicx.Ui.Pages.LocalLibrary.ViewModels;

public partial class LocalLibraryViewModel : ObservableObject
{
    private readonly ILogger<LocalLibraryViewModel> Logger;
    private readonly ISongManager _songManager;
    private readonly ISongLoader _songLoader;

    [ObservableProperty]
    private ObservableCollection<Song> _songs = [];
    
    [ObservableProperty]
    private Song? _selectedSong;

    [ObservableProperty] private bool _canGoNext = true;
    [ObservableProperty] private bool _canGoPrevious = true;
    
    [ObservableProperty] private int _currentPage = 0;
    private const int PageSize = 10000;
    
    public ICommand LoadedCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand DoubleClickCommand { get; }

    public LocalLibraryViewModel(ILoggerFactory loggerFactory, 
        ISongManager songManager,
        ISongLoader songLoader)
    {
        Logger = loggerFactory.CreateLogger<LocalLibraryViewModel>();
        _songManager = songManager;
        _songLoader = songLoader;

        NextPageCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NextPage, () => CanGoNext);
        PreviousPageCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(PreviousPage, () => CanGoPrevious);
        LoadedCommand = new RelayCommand(async() => await OnComponentLoaded());
        DoubleClickCommand = new RelayCommand(async () => await OnSongDoubleClick());
    }

    private async Task OnComponentLoaded()
    {
        await LoadSongs();
    }

    private async Task LoadSongs()
    {
        Logger.Info("⛏️ Loading songs...");
        var listSongs = await _songManager.FindAll(CurrentPage * PageSize, PageSize);
        
        Songs.Clear();
        foreach (var song in listSongs)
        {
            song.Album = await _songLoader.LoadAlbum(song.AlbumId);
            song.Artist = await _songLoader.LoadArtist(song.ArtistId);
            Songs.Add(song);
        }

        CanGoPrevious = CurrentPage > 0;
        CanGoNext = Songs.Count == PageSize;
        
        Logger.Info("✅ Songs loaded successfully!");
    }

    private Task OnSongDoubleClick()
    {
        if (null != SelectedSong)
        {
            WeakReferenceMessenger.Default.Send(new PlayAudioMessage(SelectedSong));
        }

        return Task.CompletedTask;
    }

    private void NextPage()
    {
        CurrentPage++;
        _ = LoadSongs();
    }

    private void PreviousPage()
    {
        CurrentPage--;
        _ = LoadSongs();
    }
}