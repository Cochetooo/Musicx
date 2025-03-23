using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Ui.Pages.LocalLibrary.ViewModels;
using Musicx.Ui.UIComponents.AudioPlayer.Models;
using Musicx.Ui.UIComponents.AudioPlayer.Services;
using Wpf.Ui.Controls;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace Musicx.Ui.UIComponents.AudioPlayer.ViewModels;

public partial class AudioPlayerViewModel : ObservableObject
{
    private static readonly SymbolIcon PlayIcon = new(SymbolRegular.Play24) { FontSize = 24 };
    private static readonly SymbolIcon PauseIcon = new(SymbolRegular.Pause24) { FontSize = 24 };

    private readonly AudioPlayerService _audioPlayerService;
    private readonly AudioQueueService _audioQueueService;
    private readonly DispatcherTimer _currentTrackPositionTimer;

    private readonly LocalLibraryViewModel _localLibraryViewModel;
    private readonly ILogger<AudioPlayerViewModel> Logger;

    [ObservableProperty] private Song? _currentTrack;
    [ObservableProperty] private double _currentTrackLength;
    [ObservableProperty] private double _currentTrackPosition;
    [ObservableProperty] private float _currentVolume;

    [ObservableProperty] private SymbolIcon _playbackIcon = PlayIcon;

    public AudioPlayerViewModel(
        LocalLibraryViewModel localLibraryViewModel, 
        AudioPlayerService audioPlayerService,
        AudioQueueService audioQueueService,
        ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<AudioPlayerViewModel>();

        _audioPlayerService = audioPlayerService;
        _audioQueueService = audioQueueService;
        _localLibraryViewModel = localLibraryViewModel;

        _currentTrackPositionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _currentTrackPositionTimer.Tick += CurrentTrackPositionUpdateTimer_Tick;
        
        _audioPlayerService.TrackResumed += _audioPlayer_TrackResumed;
        _audioPlayerService.TrackPaused += _audioPlayer_TrackPaused;
        _audioPlayerService.TrackEnded += _audioPlayer_TrackEnded;
        AudioEventBus.Instance.SongChanged += _audioPlayer_TrackChanged;
        
        WeakReferenceMessenger.Default.Register<PlayAudioMessage>(this, (_,m) => Play(m.song));
        
        LoadCommands();

        CurrentVolume = 1;
    }

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

    // Gestion de la file d'attente via AudioQueueService
    private void ShuffleMode() => _audioQueueService.ToggleShuffle();
    private void Previous() => _audioQueueService.Previous();
    private void Next() => _audioQueueService.Next();
    private void RepeatMode() => _audioQueueService.ToggleRepeat();

    // Gestion de la lecture via AudioPlayerService
    private void Backward() => _audioPlayerService.Seek(_audioPlayerService.GetPositionInSeconds() - 30);
    private void TogglePlayback() => _audioPlayerService.TogglePlaying();
    private void Forward() => _audioPlayerService.Seek(_audioPlayerService.GetPositionInSeconds() + 30);
    private void VolumeControlValueChanged() => _audioPlayerService.SetVolume(CurrentVolume);

    private void Play(Song song)
    {
        if (null == _localLibraryViewModel.SelectedSong)
        {
            Logger.Info("ℹ️ No song selected.");
            return;
        }
        
        Logger.Debug($"ℹ️ Selected song: {_localLibraryViewModel.SelectedSong.Title}");
        
        _audioQueueService.SetQueue(_localLibraryViewModel.Songs, _localLibraryViewModel.Songs.IndexOf(_localLibraryViewModel.SelectedSong));
    }

    private void TrackControlMouseDown() => _audioPlayerService.Pause();

    private void TrackControlMouseUp()
    {
        _audioPlayerService.SetPosition(CurrentTrackPosition);
        _audioPlayerService.Resume();
    }

    private void _audioPlayer_TrackResumed()
    {
        _currentTrackPositionTimer.Start();
        PlaybackIcon = PauseIcon;
    }

    private void _audioPlayer_TrackPaused()
    {
        _currentTrackPositionTimer.Stop();
        PlaybackIcon = PlayIcon;
    }

    private void _audioPlayer_TrackChanged(Song newTrack)
    {
        CurrentTrack = newTrack;
        CurrentTrackLength = _audioPlayerService.GetLengthInSeconds();
    }

    private void _audioPlayer_TrackEnded()
    {
        Next();
    }

    private void CurrentTrackPositionUpdateTimer_Tick(object? sender, EventArgs e)
    {
        CurrentTrackPosition = _audioPlayerService.GetPositionInSeconds();
    }
}

public record PlayAudioMessage(Song song);