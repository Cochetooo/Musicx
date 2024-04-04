package fr.xahla.musicx.desktop.views.footer.audioPlayer.trackControls;

import fr.xahla.musicx.api.data.PlayerInterface;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.manager.PlayerViewManager;
import fr.xahla.musicx.desktop.model.QueueViewModel;
import fr.xahla.musicx.desktop.model.SongViewModel;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import fr.xahla.musicx.desktop.views.footer.audioPlayer.AudioPlayerViewController;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.ProgressBar;
import javafx.scene.control.Slider;
import javafx.scene.media.Media;
import javafx.scene.media.MediaPlayer;
import javafx.scene.media.MediaView;
import javafx.scene.transform.Rotate;
import javafx.scene.transform.Translate;
import javafx.util.Duration;
import org.kordamp.ikonli.javafx.FontIcon;

import java.io.File;
import java.net.URL;
import java.util.ResourceBundle;

public class TrackControlsViewController implements ViewControllerInterface, PlayerInterface {

    public record Props(
        PlayerViewManager playerManager
    ) implements ViewControllerProps {}

    @FXML public Button playButton;
    @FXML public Slider volumeSlider;
    @FXML public Slider songTimeSlider;
    @FXML public Label currentTimeLabel;
    @FXML public Label totalTimeLabel;
    @FXML public Label volumeLabel;
    @FXML public Button volumeButton;
    @FXML private MediaView currentTrack;

    private FontIcon volumeMuteIcon, volumeOffIcon, volumeDownIcon, volumeUpIcon;
    private FontIcon playIcon, pauseIcon;

    private AudioPlayerViewController parent;
    private TrackControlsViewController.Props props;

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps viewControllerProps) {
        this.parent = (AudioPlayerViewController) viewController;
        this.props = (TrackControlsViewController.Props) viewControllerProps;

        this.volumeSlider.valueProperty().addListener((observableValue, oldValue, newValue) -> {
            if (null != props.playerManager().getMediaPlayer()) {
                props.playerManager().getPlayer().setVolume((Double) newValue);
            }

            volumeButton.setGraphic(this.getVolumeStateIcon());
            volumeLabel.setText("" + (int) ((double) newValue * 100.0));
        });

        this.songTimeSlider.valueProperty().addListener((observableValue, oldValue, newValue) -> {
            if (null != props.playerManager().getMediaPlayer()) {
                if (songTimeSlider.isValueChanging()) {
                    props.playerManager().seek(songTimeSlider.getValue());
                }
            }
        });

        this.volumeMuteIcon = new FontIcon();
        volumeMuteIcon.setIconLiteral("fas-volume-mute");
        volumeMuteIcon.setIconSize(64);

        this.volumeOffIcon = new FontIcon();
        volumeOffIcon.setIconLiteral("fas-volume-off");
        volumeOffIcon.setIconSize(64);

        this.volumeDownIcon = new FontIcon();
        volumeDownIcon.setIconLiteral("fas-volume-down");
        volumeDownIcon.setIconSize(64);

        this.volumeUpIcon = new FontIcon();
        volumeUpIcon.setIconLiteral("fas-volume-up");
        volumeUpIcon.setIconSize(64);

        this.playIcon = new FontIcon();
        playIcon.setIconLiteral("fas-play");
        playIcon.setIconSize(64);

        this.pauseIcon = new FontIcon();
        pauseIcon.setIconLiteral("fas-pause");
        pauseIcon.setIconSize(64);
    }

    @Override public AudioPlayerViewController getParent() {
        return this.parent;
    }

    @Override public void play() {
        var mediaPlayer = this.props.playerManager().getMediaPlayer();

        this.currentTrack.setMediaPlayer(mediaPlayer);

        this.props.playerManager().getMediaPlayer().currentTimeProperty().addListener(ov -> {
            var total = mediaPlayer.getTotalDuration().toMillis();
            var current = mediaPlayer.getCurrentTime().toMillis();

            songTimeSlider.setMax(total);
            songTimeSlider.setValue(current);
            currentTimeLabel.setText(DurationHelper.getTimeString(current));
            totalTimeLabel.setText(DurationHelper.getTimeString(total));
        });

        this.playButton.setGraphic(this.pauseIcon);
    }

    @Override
    public void previous() {
        this.props.playerManager().previous();
    }

    @FXML public boolean togglePlaying() {
        var playing = this.props.playerManager().togglePlaying();

        if (playing) {
            this.playButton.setGraphic(this.pauseIcon);
        } else {
            this.playButton.setGraphic(this.playIcon);
        }

        return playing;
    }

    @FXML public void stop() {
        this.props.playerManager().stop();

        this.playButton.setGraphic(this.playIcon);
    }

    @FXML public void next() {
        this.props.playerManager().next();
    }

    @FXML public void mute() {
        this.props.playerManager().mute();

        if (this.props.playerManager().getMediaPlayer().isMute()) {
            this.volumeButton.setGraphic(this.volumeMuteIcon);
        } else {
            this.volumeButton.setGraphic(this.getVolumeStateIcon());
        }
    }

    @Override public void seek(Double seconds) {

    }

    private FontIcon getVolumeStateIcon() {
        var volume = (Double) this.volumeSlider.getValue();

        if (0.5 <= volume) {
            return this.volumeUpIcon;
        } else if (0.05 <= volume) {
            return this.volumeDownIcon;
        }

        return this.volumeOffIcon;
    }

    @Override public void makeTranslations() {

    }
}
