package fr.xahla.musicx.desktop.views.audioPlayer;

import fr.xahla.musicx.desktop.config.FxmlComponent;
import fr.xahla.musicx.desktop.helper.ImageHelper;
import fr.xahla.musicx.desktop.helper.animation.ColorTransition;
import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.service.loadArtwork.LoadAlbumArtwork;
import fr.xahla.musicx.domain.helper.DurationHelper;
import fr.xahla.musicx.domain.model.enums.RepeatMode;
import javafx.application.Platform;
import javafx.beans.Observable;
import javafx.beans.value.ObservableValue;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.Slider;
import javafx.scene.effect.DropShadow;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.*;
import javafx.scene.paint.Color;
import javafx.util.Duration;
import org.kordamp.ikonli.javafx.FontIcon;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for the audio player with its controls.
 * @author Cochetooo
 * @since 0.2.0
 */
public class AudioPlayer implements Initializable {

    // Container
    @FXML private BorderPane playerContainer;

    // Album Artwork
    @FXML private DropShadow albumArtworkShadow;
    @FXML private ImageView albumArtwork;

    // Current Track Info
    @FXML private Label artistNameLabel;
    @FXML private Label trackNameLabel;

    // Player Action Buttons
    @FXML private Button togglePlayingButton;
    @FXML private Button toggleRepeatButton;
    @FXML private StackPane songRepeatBadge;

    // Current Track Time
    @FXML private Label trackTimeLabel;
    @FXML private Slider trackTimeSlider;
    @FXML private Label trackTotalTimeLabel;

    // Volume
    @FXML private Slider volumeSlider;
    @FXML private Label volumeLabel;
    @FXML private Button volumeButton;

    // Additional Icons
    private FontIcon pauseIcon, playIcon;
    private FontIcon volumeMuteIcon, volumeOffIcon, volumeDownIcon, volumeIcon, volumeUpIcon;
    private FontIcon noRepeatIcon, repeatIcon;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.setControlIcons();

        // Default values

        audioPlayer().setVolume(this.volumeSlider.getValue());

        albumArtwork.setImage(Album.artworkPlaceholder);

        // Listeners

        scene().getSettings().artworkShadowProperty().addListener(this::settingsOnArtworkShadowChange);

        audioPlayer().onCurrentTimeChange(this::playerOnCurrentTimeChange);
        audioPlayer().onSongChange(this::playerOnSongChange);
        audioPlayer().onMute(this::playerOnMute);
        audioPlayer().onPlay(() -> togglePlayingButton.setGraphic(pauseIcon));
        audioPlayer().onPause(() -> togglePlayingButton.setGraphic(playIcon));

        this.volumeSlider.valueProperty().addListener((observableValue, oldValue, newValue) -> {
            if (!audioPlayer().isPlayerInactive()) {
                audioPlayer().setVolume((Double) newValue);
            }

            this.volumeButton.setGraphic(this.getVolumeStateIcon());
            this.volumeLabel.setText("" + (int) ((double) newValue * 100.0));
        });
    }

    private void setControlIcons() {
        // Toggle Playing Button
        this.pauseIcon = new FontIcon();
        this.pauseIcon.setIconLiteral("fltfmz-pause-48");
        this.pauseIcon.setIconSize(64);

        this.playIcon = new FontIcon();
        this.playIcon.setIconLiteral("fltfmz-play-48");
        this.playIcon.setIconSize(64);

        // Volume Slider
        this.volumeMuteIcon = new FontIcon();
        this.volumeMuteIcon.setIconLiteral("fltfmz-speaker-off-48");
        this.volumeMuteIcon.setIconSize(64);

        this.volumeOffIcon = new FontIcon();
        this.volumeOffIcon.setIconLiteral("fltfmz-speaker-none-48");
        this.volumeOffIcon.setIconSize(64);

        this.volumeDownIcon = new FontIcon();
        this.volumeDownIcon.setIconLiteral("fltfmz-speaker-0-48");
        this.volumeDownIcon.setIconSize(64);

        this.volumeIcon = new FontIcon();
        this.volumeIcon.setIconLiteral("fltfmz-speaker-1-48");
        this.volumeIcon.setIconSize(64);

        this.volumeUpIcon = new FontIcon();
        this.volumeUpIcon.setIconLiteral("fltfmz-speaker-48");
        this.volumeUpIcon.setIconSize(64);

        // Repeat Mode
        this.noRepeatIcon = new FontIcon();
        this.noRepeatIcon.setIconLiteral("fltfal-arrow-repeat-all-off-24");
        this.noRepeatIcon.setIconSize(64);

        this.repeatIcon = new FontIcon();
        this.repeatIcon.setIconLiteral("fltfal-arrow-repeat-all-24");
        this.repeatIcon.setIconSize(64);
    }

    // --- Controls ---

    @FXML public void toggleShuffle() {
        audioPlayer().shuffle();
    }

    @FXML public void previous() {
        audioPlayer().previous();
    }

    @FXML public void backward() {
        audioPlayer().seek(audioPlayer().getPlayingTime() - Duration.seconds(30).toMillis());
    }

    @FXML public void togglePlaying() {
        audioPlayer().togglePlaying();
    }

    @FXML public void forward() {
        audioPlayer().seek(audioPlayer().getPlayingTime() + Duration.seconds(30).toMillis());
    }

    @FXML public void next() {
        audioPlayer().next();
    }

    @FXML public void toggleRepeat() {
        audioPlayer().toggleRepeat();

        this.toggleRepeatButton.setGraphic(audioPlayer().getRepeatMode() == RepeatMode.NO_REPEAT ? this.noRepeatIcon : this.repeatIcon);
        this.songRepeatBadge.setVisible(audioPlayer().getRepeatMode() == RepeatMode.SONG_REPEAT);
    }

    @FXML public void mute() {
        audioPlayer().mute();
    }

    // --- Listeners ---

    private void playerOnCurrentTimeChange(
        final ObservableValue<? extends Duration> observable,
        final Duration oldValue,
        final Duration newValue
    ) {
        if (null == newValue || newValue.equals(oldValue)) {
            return;
        }

        final var current = audioPlayer().getCurrentTime().toMillis();

        if (!trackTimeSlider.isValueChanging()) {
            this.trackTimeSlider.setValue(current);
        }

        this.trackTimeLabel.setText(DurationHelper.getTimeString(current));
    }

    private void playerOnMute(final Observable observable) {
        if (audioPlayer().isMuted()) {
            this.volumeButton.setGraphic(volumeMuteIcon);
        } else {
            this.volumeButton.setGraphic(this.getVolumeStateIcon());
        }
    }

    private void playerOnSongChange(final Song song) {
        this.trackTimeSlider.setDisable(false);

        this.artistNameLabel.setText((null == song.getArtist())
            ? "-"
            : song.getArtist().getName());

        this.trackNameLabel.setText(song.getTitle());

        final var total = Duration.millis(song.getDuration()).toMillis();
        this.trackTimeSlider.setMax(total);
        this.trackTotalTimeLabel.setText(DurationHelper.getTimeString(total));

        new LoadAlbumArtwork().execute(
            albumArtwork,
            song,
            this::changeBackground
        );
    }

    private void settingsOnArtworkShadowChange(
        final ObservableValue<? extends Boolean> observable,
        final Boolean oldValue,
        final Boolean newValue
    ) {
        if (newValue) {
            albumArtworkShadow.setRadius(10);
        } else {
            albumArtworkShadow.setRadius(0);
        }
    }

    @FXML private void trackTimeSliderClick(final MouseEvent event) {
        final var mouseX = event.getX();
        final var width = trackTimeSlider.getWidth();
        final var totalDuration = audioPlayer().getTotalDuration().toMillis();
        final var seekTime = (mouseX / width) * totalDuration;

        audioPlayer().seek(seekTime);
    }

    @FXML private void openQueue() {
        scene().getNavContent().switchContent(FxmlComponent.NAV_QUEUE_LIST);
    }

    // --- Helpers ---

    private FontIcon getVolumeStateIcon() {
        final var volume = this.volumeSlider.getValue();

        if (0.5 <= volume) {
            return this.volumeUpIcon;
        } else if (0.2 <= volume) {
            return this.volumeIcon;
        } else if (0.01 <= volume) {
            return this.volumeDownIcon;
        }

        return this.volumeOffIcon;
    }

    private void changeBackground(final Image image) {
        Platform.runLater(() -> {
            // Set artwork thumbnail
            albumArtwork.setImage(image);

            final var currentBackgroundColor = (Color) playerContainer.getBackground().getFills().getFirst().getFill();
            final var imageColor = ImageHelper.calculateAverageColor(image).darker();

            // Set background color
            if (scene().getSettings().isBackgroundArtworkBind()) {
                new ColorTransition(
                    Duration.seconds(0.5),
                    currentBackgroundColor,
                    imageColor,
                    10,
                    (color) -> {
                        final var newBackground = new Background(new BackgroundFill(color, CornerRadii.EMPTY, null));
                        playerContainer.setBackground(newBackground);
                    }
                ).play();
            } else {
                playerContainer.setBackground(Background.fill(Color.TRANSPARENT));
            }

            // Set shadow artwork
            if (scene().getSettings().isArtworkShadow()) {
                albumArtworkShadow.setColor(imageColor.brighter());
            }
        });
    }
}
