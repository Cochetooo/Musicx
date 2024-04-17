package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.DurationHelper;
import javafx.beans.value.ChangeListener;
import javafx.beans.value.ObservableValue;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.Slider;
import javafx.scene.media.MediaView;
import javafx.util.Duration;
import org.kordamp.ikonli.javafx.FontIcon;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;
import static fr.xahla.musicx.desktop.DesktopContext.player;

/** <b>View for the audio player with its controls.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Player implements Initializable {

    @FXML private MediaView trackMediaView;

    @FXML private Label artistNameLabel;
    @FXML private Label trackNameLabel;

    @FXML private Button previousButton;
    @FXML private Button backwardButton;
    @FXML private Button togglePlayingButton;
    @FXML private Button forwardButton;
    @FXML private Button nextButton;

    @FXML private Label trackTimeLabel;
    @FXML private Slider trackTimeSlider;
    @FXML private Label trackTotalTimeLabel;

    @FXML private Slider volumeSlider;
    @FXML private Label volumeLabel;
    @FXML private Button volumeButton;

    private FontIcon pauseIcon, playIcon;
    private FontIcon volumeMuteIcon, volumeOffIcon, volumeDownIcon, volumeIcon, volumeUpIcon;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.setControlIcons();

        this.volumeSlider.valueProperty().addListener((observableValue, oldValue, newValue) -> {
            if (!player().isPlayerInactive()) {
                player().setVolume((Double) newValue);
            }

            this.volumeButton.setGraphic(this.getVolumeStateIcon());
            this.volumeLabel.setText("" + (int) ((double) newValue * 100.0));
        });

        this.trackTimeSlider.setOnMouseClicked(event -> {
            final var mouseX = event.getX();
            final var width = trackTimeSlider.getWidth();
            final var totalDuration = player().getTotalDuration().toMillis();
            final var seekTime = (mouseX / width) * totalDuration;

            player().seek(seekTime);
        });

        player().onCurrentTimeChange((observable, oldValue, newValue) -> {
            if (null == newValue || newValue.equals(oldValue)) {
                return;
            }

            final var current = player().getCurrentTime().toMillis();

            if (!trackTimeSlider.isValueChanging()) {
                this.trackTimeSlider.setValue(current);
            }

            this.trackTimeLabel.setText(DurationHelper.getTimeString(current));
        });

        player().onSongChange((song) -> {
            this.trackTimeSlider.setDisable(false);

            this.artistNameLabel.setText(song.getArtist().getName());
            this.trackNameLabel.setText(song.getTitle());

            final var total = Duration.seconds(song.getDuration()).toMillis();
            this.trackTimeSlider.setMax(total);
            this.trackTotalTimeLabel.setText(DurationHelper.getTimeString(total));
        });

        player().onMute((observable) -> {
            if (player().isMuted()) {
                this.volumeButton.setGraphic(volumeMuteIcon);
            } else {
                this.volumeButton.setGraphic(this.getVolumeStateIcon());
            }
        });

        player().onPlay(() -> togglePlayingButton.setGraphic(pauseIcon));
        player().onPause(() -> togglePlayingButton.setGraphic(playIcon));
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
    }

    // --- Controls ---

    @FXML public void previous() {
        player().previous();
    }

    @FXML public void backward() {
        player().seek(player().getPlayingTime() - Duration.seconds(30).toMillis());
    }

    @FXML public void togglePlaying() {
        player().togglePlaying();
    }

    @FXML public void forward() {
        player().seek(player().getPlayingTime() + Duration.seconds(30).toMillis());
    }

    @FXML public void next() {
        player().next();
    }

    @FXML public void mute() {
        player().mute();
    }

    // --- Helpers ---

    private FontIcon getVolumeStateIcon() {
        final var volume = (Double) this.volumeSlider.getValue();

        if (0.65 <= volume) {
            return this.volumeUpIcon;
        } else if (0.3 <= volume) {
            return this.volumeIcon;
        } else if (0.05 <= volume) {
            return this.volumeDownIcon;
        }

        return this.volumeOffIcon;
    }
}
