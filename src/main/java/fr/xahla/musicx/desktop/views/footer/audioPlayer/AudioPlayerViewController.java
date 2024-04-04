package fr.xahla.musicx.desktop.views.footer.audioPlayer;

import fr.xahla.musicx.desktop.manager.PlayerViewManager;
import fr.xahla.musicx.desktop.model.QueueViewModel;
import fr.xahla.musicx.desktop.model.SongViewModel;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.desktop.views.MusicxViewController;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import fr.xahla.musicx.desktop.views.footer.audioPlayer.trackControls.TrackControlsViewController;
import fr.xahla.musicx.desktop.views.footer.audioPlayer.trackInfo.TrackInfoViewController;
import javafx.fxml.FXML;

public class AudioPlayerViewController implements ViewControllerInterface {

    public record Props(
        PlayerViewManager playerManager
    ) implements ViewControllerProps {}

    @FXML private TrackControlsViewController trackControlsViewController;
    @FXML private TrackInfoViewController trackInfoViewController;

    private MusicxViewController parent;
    private AudioPlayerViewController.Props props;

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps props) {
        this.parent = (MusicxViewController) viewController;
        this.props = (AudioPlayerViewController.Props) props;

        this.trackControlsViewController.initialize(this,
            new TrackControlsViewController.Props(
                this.props.playerManager()
            ));

        this.trackInfoViewController.initialize(this,
            new TrackInfoViewController.Props(
                this.props.playerManager()
            ));
    }

    @Override public void makeTranslations() {

    }

    @Override public MusicxViewController getParent() {
        return this.parent;
    }

    public TrackControlsViewController getTrackControlsViewController() {
        return this.trackControlsViewController;
    }
}
