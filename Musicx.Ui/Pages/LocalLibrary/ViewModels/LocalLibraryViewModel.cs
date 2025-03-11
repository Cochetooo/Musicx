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
    private readonly ILogger<LocalLibraryViewModel> Logger;
    private readonly IManager<Song> _songManager;

    [ObservableProperty]
    private ObservableCollection<Song> _songs = [];
    
    [ObservableProperty]
    private Song? _selectedSong;

    [ObservableProperty] private bool _canGoNext = true;
    [ObservableProperty] private bool _canGoPrevious = true;
    
    [ObservableProperty] private int _currentPage = 0;
    private const int PageSize = 50;
    
    public ICommand LoadedCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }

    public LocalLibraryViewModel(ILoggerFactory loggerFactory, ISongManager songManager)
    {
        Logger = loggerFactory.CreateLogger<LocalLibraryViewModel>();
        _songManager = songManager;

        NextPageCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NextPage, () => CanGoNext);
        PreviousPageCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(PreviousPage, () => CanGoPrevious);
        LoadedCommand = new RelayCommand(async() => await OnComponentLoaded());
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
            Songs.Add(song);
        }

        CanGoPrevious = CurrentPage > 0;
        CanGoNext = Songs.Count == PageSize;
        
        Logger.Info("✅ Songs loaded successfully!");
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