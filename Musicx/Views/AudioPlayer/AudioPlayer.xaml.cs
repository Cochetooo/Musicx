using System.Windows.Controls;
using log4net;

namespace Musicx.Views.AudioPlayer;

public partial class AudioPlayer
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AudioPlayer));
    
    public AudioPlayer()
    {
        InitializeComponent();
    }
}