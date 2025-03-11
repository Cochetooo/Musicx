using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Ui.Pages.LocalLibrary.ViewModels;
using Musicx.Ui.UIComponents.AudioPlayer.Models;
using Musicx.Ui.UIComponents.AudioPlayer.Services;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace Musicx.Ui.UIComponents.AudioPlayer.ViewModels;

public partial class AudioPlayerViewModel : ObservableObject
{
    private readonly ILogger<AudioPlayerViewModel> Logger;
    
    [ObservableProperty] private Song? _currentTrack;

    [ObservableProperty] private double _currentTrackLength;
    [ObservableProperty] private double _currentTrackPosition;
    [ObservableProperty] private float _currentVolume;
    
    [ObservableProperty] private AudioPlayerService _audioPlayerService;
    private readonly DispatcherTimer _currentTrackPositionTimer;
    
    public ICommand ShuffleModeCommand { get; private set; }
    public ICommand PreviousCommand { get; private set; }
    public ICommand BackwardCommand { get; private set; }
    public ICommand PlaybackCommand { get; private set; }
    public ICommand ForwardCommand { get; private set; }
    public ICommand NextCommand { get; private set; }
    public ICommand RepeatModeCommand { get; private set; }
    
    public ICommand TrackControlMouseDownCommand { get; private set; }
    public ICommand TrackControlMouseUpCommand { get; private set; }
    public ICommand VolumeControlValueChangedCommand { get; private set; }
    
    private readonly LocalLibraryViewModel _localLibraryViewModel;

    public AudioPlayerViewModel(LocalLibraryViewModel localLibraryViewModel, ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<AudioPlayerViewModel>();

        _audioPlayerService = new AudioPlayerService(localLibraryViewModel.Songs);
        
        _localLibraryViewModel = localLibraryViewModel;

        _currentTrackPositionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _currentTrackPositionTimer.Tick += CurrentTrackPositionUpdateTimer_Tick;
        
        _audioPlayerService.TrackResumed += () => _currentTrackPositionTimer.Start();
        _audioPlayerService.TrackPaused += () => _currentTrackPositionTimer.Stop();
        
        LoadCommands();

        CurrentVolume = 1;
    }

    private void LoadCommands()
    {
        ShuffleModeCommand = new RelayCommand(ShuffleMode);
        PreviousCommand = new RelayCommand(Previous);
        BackwardCommand = new RelayCommand(Backward);
        PlaybackCommand = new RelayCommand(TogglePlayback);
        ForwardCommand = new RelayCommand(Forward);
        NextCommand = new RelayCommand(Next);
        RepeatModeCommand = new RelayCommand(RepeatMode);
        
        TrackControlMouseDownCommand = new RelayCommand(TrackControlMouseDown);
        TrackControlMouseUpCommand = new RelayCommand(TrackControlMouseUp);
        VolumeControlValueChangedCommand = new RelayCommand(VolumeControlValueChanged);
    }

    private void ShuffleMode()
    {
        AudioPlayerService.Shuffle();
    }

    private void Previous()
    {
        AudioPlayerService.Previous();
    }

    private void Backward()
    {
        AudioPlayerService.Seek(AudioPlayerService.GetPositionInSeconds() - 30);
    }
    
    private void TogglePlayback()
    {
        if (null == _localLibraryViewModel.SelectedSong)
        {
            Logger.Info("ℹ️ No song selected.");
            return;
        }
        
        Logger.Debug($"ℹ️ Selected song: {_localLibraryViewModel.SelectedSong.Title}");
        
        AudioPlayerService.SetQueue(_localLibraryViewModel.Songs, _localLibraryViewModel.Songs.IndexOf(_localLibraryViewModel.SelectedSong));
        CurrentTrackLength = AudioPlayerService.GetLengthInSeconds();
        CurrentTrack = _localLibraryViewModel.SelectedSong;
    }
    
    private void Forward()
    {
        AudioPlayerService.Seek(AudioPlayerService.GetPositionInSeconds() + 30);
    }
    
    private void Next()
    {
        AudioPlayerService.Next();
    }
    
    private void RepeatMode()
    {
        AudioPlayerService.ToggleRepeat();
    }
    
    private void TrackControlMouseDown()
    {
        AudioPlayerService.Pause();
    }
    
    private void TrackControlMouseUp()
    {
        AudioPlayerService.SetPosition(CurrentTrackPosition);
        AudioPlayerService.Resume();
    }
    
    private void VolumeControlValueChanged()
    {
        AudioPlayerService.SetVolume(CurrentVolume);
    }

    private void CurrentTrackPositionUpdateTimer_Tick(object? sender, EventArgs e)
    {
        CurrentTrackPosition = AudioPlayerService.GetPositionInSeconds();
    }
}