using log4net;

namespace Musicx.Ui.UIComponents.AudioPlayer.Views;

public partial class AudioPlayerView
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AudioPlayerView));
    
    public AudioPlayerView()
    {
        InitializeComponent();
    }
}