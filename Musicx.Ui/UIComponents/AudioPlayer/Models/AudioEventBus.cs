using Musicx.Core.Models;

namespace Musicx.Ui.UIComponents.AudioPlayer.Models;

public class AudioEventBus
{
    private static readonly Lazy<AudioEventBus> _instance = new(() => new AudioEventBus());
    public static AudioEventBus Instance => _instance.Value;

    public event Action<Song>? SongChanged;

    public void TriggerSongChanged(Song song)
    {
        SongChanged?.Invoke(song);
    }
}