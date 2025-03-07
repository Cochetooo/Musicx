using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Musicx.Core.Models;
using Musicx.Infrastructure.Managers;

namespace Musicx.Ui.Views.LocalLibrary;

public partial class SongList : UserControl
{
    private const int Buffer = 50;
    private const int PageSize = 200;
    
    private readonly SongManager _songManager;

    private ObservableCollection<Song> _songs;
    public ObservableCollection<Song> Songs
    {
        get => _songs;
        set
        {
            _songs = value;
            OnPropertyChanged();
        }
    }

    private bool _isLoading;
    
    public SongList(SongManager songManager)
    {
        InitializeComponent();
        _songManager = songManager;
        _songs = [];

        Loaded += async (_,_) => await LoadInitialSongs();
    }

    private void SongList_OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is not DataGrid)
        {
            return;
        }
        
        var verticalOffset = e.VerticalOffset;
        var extentHeight = e.ExtentHeight;
        var viewportHeight = e.ViewportHeight;
        var ratioScrolled = verticalOffset / (extentHeight - viewportHeight);
        
        
    }
    
    private async Task LoadInitialSongs()
    {
        var initialSongs = await _songManager.GetAll(0, PageSize);
        foreach (var song in initialSongs)
        {
            Songs.Add(song);
        }
    }

    public async Task LoadMoreSongs()
    {
        if (_isLoading) return;
        _isLoading = true;
        
        var nextSongs = await _songManager.GetAll(Songs.Count, PageSize);
        foreach (var song in nextSongs)
        {
            Songs.Add(song);
        }
        
        _isLoading = false;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}