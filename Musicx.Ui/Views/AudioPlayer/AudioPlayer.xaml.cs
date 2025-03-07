using log4net;

namespace Musicx.Ui.Views.AudioPlayer;

public partial class AudioPlayer
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AudioPlayer));
    
    public AudioPlayer()
    {
        InitializeComponent();
    }
}