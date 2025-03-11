using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using Musicx.Core.Models;
using Musicx.Ui.UIComponents.AudioPlayer.Models;
using NAudio.Wave;

namespace Musicx.Ui.UIComponents.AudioPlayer.Services;

public class AudioPlayerService
{
    public enum PlaybackStopTypes
    {
        PlaybackStoppedByUser, PlaybackStoppedReachingEndOfFile
    }
    
    public PlaybackStopTypes PlaybackStopType { get; set; }

    private AudioFileReader? _audioFileReader;
    private DirectSoundOut? _output;
    
    private AudioQueueService _audioQueueService;
    private AudioPlayerData _audioPlayerData;

    public event Action? TrackResumed;
    public event Action? TrackPaused;
    public event Action? TrackMuted;
    public event Action? TrackPositionChanged;
    public event Action? MediaChanged;

    public AudioPlayerService(ObservableCollection<Song> songs)
    {
        PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;

        _audioQueueService = new AudioQueueService();
        _audioPlayerData = new AudioPlayerData();
        
        _audioQueueService.SetQueue(songs, 0);

        _audioQueueService.AudioQueue.IndexChanged += (sender, args) =>
        {
            _audioPlayerData.Song = _audioQueueService[args.NewIndex];
            UpdateSong();
            MediaChanged?.Invoke();
        };
    }
    
    public void ClearQueue() => _audioQueueService.Clear();
    public void QueueLast(Song song) => _audioQueueService.QueueLast(song);
    public void QueueNext(Song song) => _audioQueueService.QueueNext(song);
    public void SetCurrentTrackByIndex(int index) => _audioQueueService.SetIndex(index);
    public void SetQueue(ICollection<Song> songs, int index) => _audioQueueService.SetQueue(songs, index);
    public long GetTotalQueueDurationInSeconds() => _audioQueueService.AudioQueue.QueueDuration;

    public void UpdateSong()
    {
        if (null == _audioPlayerData.Song)
        {
            return;
        }
        
        var filePath = _audioPlayerData.Song.Filepath;

        _output?.Stop();
        
        _audioFileReader = new AudioFileReader(filePath);
        
        _output = new DirectSoundOut(200);
        _audioFileReader = new AudioFileReader(filePath);
        
        var wc = new WaveChannel32(_audioFileReader)
        {
            PadWithZeroes = false
        };
        
        _output?.Init(wc);
        Resume();
    }

    public void Mute()
    {
        throw new NotImplementedException();
    }

    public void Next()
    {
        switch (_audioPlayerData.RepeatMode)
        {
            case RepeatMode.NoRepeat: 
                _audioQueueService.MoveNext();
                break;
            
            case RepeatMode.QueueRepeat:
                if (_audioQueueService.IsLastSong())
                {
                    _audioQueueService.SetIndex(0);
                }
                else
                {
                    _audioQueueService.MoveNext();
                }

                break;
            
            case RepeatMode.SongRepeat:
                _audioQueueService.RepeatSong();
                break;
        }
    }

    public void Pause()
    {
        var startVolume = GetVolume();

        var volumeAnimation = new DoubleAnimation()
        {
            From = startVolume,
            To = 0.0,
            Duration = TimeSpan.FromSeconds(0.35),
            FillBehavior = FillBehavior.Stop
        };

        volumeAnimation.Completed += (s, e) =>
        {
            _output?.Pause();
            SetVolume(startVolume);
        };
        
        TrackPaused?.Invoke();
    }

    public void Previous()
    {
        _audioQueueService.MovePrevious();
    }

    public void Remove(int index)
    {
        _audioQueueService.Remove(index);
    }

    public void Resume()
    {
        _output?.Play();
        TrackResumed?.Invoke();
    }

    public void Seek(double seconds)
    {
        throw new NotImplementedException();
    }

    public void Shuffle()
    {
        _audioQueueService.Shuffle();
    }

    public void Stop()
    {
        _output?.Stop();
        _audioQueueService.Clear();
    }

    public void TogglePlaying()
    {
        if (IsPlaying())
        {
            Pause();
            return;
        }

        Resume();
    }

    public void ToggleRepeat()
    {
        _audioPlayerData.RepeatMode = _audioPlayerData.RepeatMode switch
        {
            RepeatMode.NoRepeat => RepeatMode.QueueRepeat,
            RepeatMode.QueueRepeat => RepeatMode.SongRepeat,
            RepeatMode.SongRepeat => RepeatMode.NoRepeat,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public bool IsPlaying()
    {
        return _output?.PlaybackState == PlaybackState.Playing;
    }

    public double GetLengthInSeconds()
    {
        if (null != _audioFileReader)
        {
            return _audioFileReader.TotalTime.TotalSeconds;
        }

        return 0;
    }

    public double GetPositionInSeconds()
    {
        return null != _audioFileReader ? _audioFileReader.CurrentTime.TotalSeconds : 0;
    }

    public float GetVolume()
    {
        if (null != _audioFileReader)
        {
            return _audioFileReader.Volume;
        }

        return 1;
    }

    public void SetPosition(double value)
    {
        if (null != _audioFileReader)
        {
            _audioFileReader.CurrentTime = TimeSpan.FromSeconds(value);
        }
    }

    public void SetVolume(float value)
    {
        if (null != _audioFileReader)
        {
            _audioFileReader.Volume = value;
        }
    }
}