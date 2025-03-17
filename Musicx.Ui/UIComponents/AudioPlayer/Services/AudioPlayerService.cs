using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using Musicx.Core.Models;
using Musicx.Ui.UIComponents.AudioPlayer.Models;
using NAudio.Wave;

namespace Musicx.Ui.UIComponents.AudioPlayer.Services;

public class AudioPlayerService
{
    private AudioFileReader? _audioFileReader;
    private DirectSoundOut? _output;
    private WaveChannel32? _waveChannel;
    
    public bool IsMuted { get; private set; }
    private float _previousVolume = 1.0f;

    public event Action? TrackResumed;
    public event Action? TrackPaused;
    public event Action? TrackMuted;

    public AudioPlayerService()
    {
        AudioEventBus.Instance.SongChanged += OnSongChanged;
    }

    private void OnSongChanged(Song newSong)
    {
        Play(newSong.Filepath);
    }

    private void Play(string filePath)
    {
        Stop();
        var previousVolume = _waveChannel?.Volume ?? 1.0f;
        _audioFileReader = new AudioFileReader(filePath);
        _waveChannel = new WaveChannel32(_audioFileReader) { PadWithZeroes = false };
        _output = new DirectSoundOut(100);
        _output.Init(_waveChannel);
        SetVolume(previousVolume);
        
        Resume();
    }

    public void Mute()
    {
        if (null == _waveChannel)
        {
            return;
        }
        
        if (IsMuted)
        {
            _waveChannel.Volume = _previousVolume;
        }
        else
        {
            _previousVolume = _waveChannel.Volume;
            _waveChannel.Volume = 0;
        }
        
        IsMuted = !IsMuted;
        TrackMuted?.Invoke();
    }

    public void Pause()
    {
        /* var startVolume = GetVolume();

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
        }; */

        _output?.Pause();
        
        TrackPaused?.Invoke();
    }

    public void Resume()
    {
        _output?.Play();
        TrackResumed?.Invoke();
    }

    public void Seek(double seconds)
    {
        if (null == _audioFileReader || seconds < 0)
        {
            return;
        }

        long newPosition = (long)(seconds * _audioFileReader.WaveFormat.AverageBytesPerSecond);
        newPosition = Math.Min(newPosition, _audioFileReader.Length);
        
        _audioFileReader.Position = newPosition;
    }

    public void Stop()
    {
        _output?.Stop();
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

    public void SetPosition(double value)
    {
        if (null != _audioFileReader)
        {
            _audioFileReader.CurrentTime = TimeSpan.FromSeconds(value);
        }
    }

    public void SetVolume(float value)
    {
        if (null != _waveChannel)
        {
            _waveChannel.Volume = value;
        }
    }
}