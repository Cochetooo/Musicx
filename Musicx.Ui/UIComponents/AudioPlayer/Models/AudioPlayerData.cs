using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Core.Models;

namespace Musicx.Ui.UIComponents.AudioPlayer.Models;

public partial class AudioPlayerData : ObservableObject
{
    [ObservableProperty] private double _volume;
    [ObservableProperty] private Song? _song;
    [ObservableProperty] private ShuffleMode _shuffleMode;
    [ObservableProperty] private RepeatMode _repeatMode;
}

public enum ShuffleMode
{
    /// <summary>
    /// Play the queue in order.
    /// </summary>
    Linear,
    
    /// <summary>
    ///  Shuffle the queue and play tracks randomly.
    /// </summary>
    Shuffle,
}

public enum RepeatMode
{
    /// <summary>
    /// Do not repeat when the queue has no song left.
    /// </summary>
    NoRepeat,
    
    /// <summary>
    /// Repeat the entire queue when the queue has no song left.
    /// </summary>
    QueueRepeat,
    
    /// <summary>
    /// Repeat the current song.
    /// </summary>
    SongRepeat,
}