package fr.xahla.musicx.desktop.views.footer.audioPlayer;

import fr.xahla.musicx.desktop.model.SongViewModel;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.desktop.views.MusicxViewController;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import fr.xahla.musicx.desktop.views.footer.audioPlayer.trackControls.TrackControlsViewController;
import fr.xahla.musicx.desktop.views.footer.audioPlayer.trackInfo.TrackInfoViewController;
import javafx.fxml.FXML;

public class AudioPlayerViewController implements ViewControllerInterface {

    @FXML private TrackControlsViewController trackControlsViewController;
    @FXML private TrackInfoViewController trackInfoViewController;

    private MusicxViewController parent;
    private SongViewModel currentSong;

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps props) {
        this.parent = (MusicxViewController) viewController;

        this.currentSong = new SongViewModel();

        this.trackControlsViewController.initialize(this,
            new TrackControlsViewController.Props(
                this.currentSong
            ));

        this.trackInfoViewController.initialize(this,
            new TrackInfoViewController.Props(
                this.currentSong
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

    public void setCurrentPlayingSong(SongViewModel song) {
        this.currentSong.set(song);
    }
}
