using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Core.Models;

namespace Musicx.Ui.UIComponents.AudioPlayer.Models;

public class AudioQueue
{
    public int Index;
    public ObservableCollection<Song> Songs = [];
}