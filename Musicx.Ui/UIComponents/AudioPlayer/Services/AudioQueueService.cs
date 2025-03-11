using Musicx.Core.Models;
using Musicx.Ui.UIComponents.AudioPlayer.Models;

namespace Musicx.Ui.UIComponents.AudioPlayer.Services;

public class AudioQueueService
{
    public readonly AudioQueue AudioQueue;

    public AudioQueueService()
    {
        AudioQueue = new AudioQueue();

        AudioQueue.Songs.CollectionChanged += (_,_) =>
        {
            AudioQueue.QueueDuration = AudioQueue.Songs
                .Select(s => s.Duration)
                .Sum();
        };
    }

    public void Clear()
    {
        AudioQueue.Songs.Clear();
        SetIndex(-1);
    }
    
    public bool IsLastSong() => AudioQueue.QueueIndex == AudioQueue.Songs.Count - 1;

    public void MovePrevious()
    {
        SetIndex(AudioQueue.QueueIndex - 1);
    }

    public void MoveNext()
    {
        SetIndex(AudioQueue.QueueIndex + 1);
    }

    public void QueueNext(Song song)
    {
        AudioQueue.Songs.Insert(AudioQueue.QueueIndex + 1, song);
    }

    public void QueueLast(Song song)
    {
        AudioQueue.Songs.Add(song);
    }

    public void Remove(int index)
    {
        if (index < AudioQueue.QueueIndex)
        {
            return;
        }

        if (index == AudioQueue.QueueIndex)
        {
            MoveNext();
        }
        
        AudioQueue.Songs.RemoveAt(index);
    }

    public void RepeatSong()
    {
        throw new NotImplementedException();
    }

    public void Shuffle()
    {
        throw new NotImplementedException();
    }

    public void SetQueue(ICollection<Song> songs, int index)
    {
        AudioQueue.Songs.Clear();
        
        foreach (var song in songs)
        {
            AudioQueue.Songs.Add(song);
        }

        SetIndex(index);
    }

    public void SetIndex(int index)
    {
        if (0 == AudioQueue.Songs.Count)
        {
            AudioQueue.QueueIndex = -1;
            return;
        }

        if (index >= AudioQueue.Songs.Count || index < 0)
        {
            return;
        }
            
        AudioQueue.QueueIndex = index;
        AudioQueue.OnIndexChanged(index);
    }
    
    public Song? this[int index]
    {
        get {
            if (index < 0 || index >= AudioQueue.Songs.Count) {
                return null;
            }

            return AudioQueue.Songs[index];
        }
    }
}