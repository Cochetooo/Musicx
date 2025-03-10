using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Core.Models;

namespace Musicx.Ui.UIComponents.AudioPlayer.Models;

public partial class AudioQueue : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Song> _songs = [];
    [ObservableProperty] private long _queueDuration;
    [ObservableProperty] private int _queueIndex;

    public event EventHandler<IndexChangedArgs>? IndexChanged;

    public void OnIndexChanged(int index)
    {
        IndexChanged?.Invoke(this, new IndexChangedArgs { PreviousIndex = QueueIndex, NewIndex = index });
    }

    public class IndexChangedArgs
    {
        public int PreviousIndex { get; set; }
        public int NewIndex { get; set; }
    }
}