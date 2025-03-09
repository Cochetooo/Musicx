using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Accessibility;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Managers;
using Musicx.Ui.Commands;
using NAudio.Wave;
using Wpf.Ui.Input;

namespace Musicx.Ui.ViewModels.LocalLibrary;

public class SongListViewModel
{
    private readonly ILogger Logger;

    private readonly IManager<Song> _songManager;

    public ObservableCollection<Song> Songs { get; set; } = [];
    
    public ICommand LoadedCommand { get; }
    public ICommand PlaySongCommand { get; }

    private IWavePlayer? waveOut;
    private AudioFileReader? audioFileReader;

    public SongListViewModel(ILoggerFactory loggerFactory, ISongManager songManager)
    {
        Logger = loggerFactory.CreateLogger(typeof(SongListViewModel));
        _songManager = songManager;
        
        LoadedCommand = new RelayCommand(async() => await OnComponentLoaded());
        PlaySongCommand = new RelayCommand<Song>(PlaySong);
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

    private void PlaySong(Song? song)
    {
        if (null == song || string.IsNullOrEmpty(song.Filepath))
        {
            Logger.Warn("⚠️ Song is null or filepath is empty.");
            return;
        }
        
        Logger.Debug($"⛏️ Playing song {song.Filepath}...");

        StopPlayback();

        try
        {
            audioFileReader = new AudioFileReader(song.Filepath);
            audioFileReader.Volume = 0.5f;
            waveOut = new WaveOutEvent();
            waveOut.Volume = 0.5f;
            waveOut.Init(audioFileReader);
            waveOut.Play();

            waveOut.PlaybackStopped += (s, e) => StopPlayback();
        }
        catch (Exception ex)
        {
            Logger.Fatal($"❌ Error playing song {song.Filepath}", ex);
        }
    }

    private void StopPlayback()
    {
        if (null != waveOut)
        {
            Logger.Debug("ℹ️ Emptying waveOut");
            waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;
        }

        if (null != audioFileReader)
        {
            Logger.Debug("ℹ️ Emptying AudioFileReader");
            audioFileReader.Dispose();
            audioFileReader = null;
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}