package fr.xahla.musicx.desktop.views.footer.audioPlayer.trackInfo;

import fr.xahla.musicx.desktop.model.SongViewModel;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import fr.xahla.musicx.desktop.views.footer.audioPlayer.AudioPlayerViewController;
import javafx.fxml.FXML;
import javafx.scene.control.Label;

public class TrackInfoViewController implements ViewControllerInterface {
    public record Props(
        SongViewModel currentSong
    ) implements ViewControllerProps {}

    @FXML public Label displayArtistName;
    @FXML public Label displayTrackName;

    private AudioPlayerViewController parent;
    private TrackInfoViewController.Props props;

    @Override public AudioPlayerViewController getParent() {
        return this.parent;
    }

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps viewControllerProps) {
        this.parent = (AudioPlayerViewController) viewController;
        this.props = (TrackInfoViewController.Props) viewControllerProps;

        this.displayTrackName.textProperty().bind(this.props.currentSong().titleProperty());
        this.displayArtistName.textProperty().bind(this.props.currentSong().getArtist().nameProperty());
    }

    @Override public void makeTranslations() {

    }
}
