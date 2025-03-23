using System.Collections.Immutable;
using Musicx.Core.Models;
using Musicx.Ui.UIComponents.AudioPlayer.Models;

namespace Musicx.Ui.UIComponents.AudioPlayer.Services;

public class AudioQueueService
{
    private readonly AudioQueue _queue = new();

    public ImmutableList<Song> Songs => _queue.Songs.ToImmutableList();

    public Song? this[int index]
    {
        get {
            if (index < 0 || index >= _queue.Songs.Count) {
                return null;
            }

            return _queue.Songs[index];
        }
    }

    public void Clear()
    {
        _queue.Songs.Clear();
        SetIndex(-1);
    }

    public bool IsLastSong() => _queue.Index == _queue.Songs.Count - 1;

    public void Next()
    {
        if (_queue.Index < _queue.Songs.Count - 1)
        {
            SetIndex(_queue.Index + 1);
        }
    }

    public void Previous()
    {
        if (_queue.Index > 0)
        {
            SetIndex(_queue.Index - 1);
        }
    }

    public void AddSongNext(Song song)
    {
        _queue.Songs.Insert(_queue.Index + 1, song);
    }

    public void AddSongLast(Song song)
    {
        _queue.Songs.Add(song);
    }

    public void Remove(int index)
    {
        if (index < _queue.Index)
        {
            return;
        }

        if (index == _queue.Index)
        {
            Next();
        }
        
        _queue.Songs.RemoveAt(index);
    }

    public void SetQueue(ICollection<Song> songs, int index)
    {
        _queue.Songs.Clear();
        
        foreach (var song in songs)
        {
            _queue.Songs.Add(song);
        }

        SetIndex(index);
    }

    public void SetIndex(int index)
    {
        if (index >= 0 && index < _queue.Songs.Count)
        {
            _queue.Index = index;
            AudioEventBus.Instance.TriggerSongChanged(_queue.Songs[index]);
        }
    }

    public void ToggleShuffle()
    {
        throw new NotImplementedException();
    }

    public void ToggleRepeat()
    {
        throw new NotImplementedException();
    }
}