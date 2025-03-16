using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Core.Models;

namespace Musicx.Ui.UIComponents.AudioPlayer.Models;

public class AudioQueue
{
    public ObservableCollection<Song> Songs = [];
    public int Index;
}