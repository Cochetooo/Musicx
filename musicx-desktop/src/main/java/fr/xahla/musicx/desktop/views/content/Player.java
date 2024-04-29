package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.infrastructure.service.albumArtwork.GetArtworkFromiTunes;
import fr.xahla.musicx.desktop.DesktopApplication;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.helper.ImageHelper;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.model.enums.RepeatMode;
import javafx.animation.FadeTransition;
import javafx.application.Platform;
import javafx.beans.Observable;
import javafx.beans.value.ObservableValue;
import javafx.concurrent.Task;
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
import javafx.util.Duration;
import org.kordamp.ikonli.javafx.FontIcon;

import java.net.URISyntaxException;
import java.net.URL;
import java.util.Objects;
import java.util.ResourceBundle;

import static fr.xahla.musicx.domain.application.AbstractContext.lastFmApi;
import static fr.xahla.musicx.desktop.DesktopContext.player;
import static fr.xahla.musicx.desktop.DesktopContext.settings;

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

    // Container
    @FXML private BorderPane playerContainer;

    // Album Artwork
    @FXML private DropShadow albumArtworkShadow;
    @FXML private ImageView albumArtwork;
    private String albumThumbnailPlaceholderImageURL;

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

        player().setVolume(this.volumeSlider.getValue());

        try {
            this.albumThumbnailPlaceholderImageURL =
                Objects.requireNonNull(DesktopApplication.class.getResource("assets/images/thumbnail-placeholder.png"))
                    .toURI()
                    .toString();
        } catch (final URISyntaxException e) {
            throw new RuntimeException(e);
        }

        albumArtwork.setImage(new Image(albumThumbnailPlaceholderImageURL));

        // Listeners

        settings().artworkShadowProperty().addListener(this::settingsOnArtworkShadowChange);

        player().onCurrentTimeChange(this::playerOnCurrentTimeChange);
        player().onSongChange(this::playerOnSongChange);
        player().onMute(this::playerOnMute);
        player().onPlay(() -> togglePlayingButton.setGraphic(pauseIcon));
        player().onPause(() -> togglePlayingButton.setGraphic(playIcon));

        this.volumeSlider.valueProperty().addListener((observableValue, oldValue, newValue) -> {
            if (!player().isPlayerInactive()) {
                player().setVolume((Double) newValue);
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
        player().shuffle();
    }

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

    @FXML public void toggleRepeat() {
        player().toggleRepeat();

        this.toggleRepeatButton.setGraphic(player().getRepeatMode() == RepeatMode.NO_REPEAT ? this.noRepeatIcon : this.repeatIcon);
        this.songRepeatBadge.setVisible(player().getRepeatMode() == RepeatMode.SONG_REPEAT);
    }

    @FXML public void mute() {
        player().mute();
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

        final var current = player().getCurrentTime().toMillis();

        if (!trackTimeSlider.isValueChanging()) {
            this.trackTimeSlider.setValue(current);
        }

        this.trackTimeLabel.setText(DurationHelper.getTimeString(current));
    }

    private void playerOnMute(final Observable observable) {
        if (player().isMuted()) {
            this.volumeButton.setGraphic(volumeMuteIcon);
        } else {
            this.volumeButton.setGraphic(this.getVolumeStateIcon());
        }
    }

    private void playerOnSongChange(final Song song) {
        this.trackTimeSlider.setDisable(false);

        this.artistNameLabel.setText(song.getArtist().getName());
        this.trackNameLabel.setText(song.getTitle());

        final var total = Duration.seconds(song.getDuration()).toMillis();
        this.trackTimeSlider.setMax(total);
        this.trackTotalTimeLabel.setText(DurationHelper.getTimeString(total));

        final var getArtworkTask = new Task<>() {
            @Override protected Void call() {
                // We try to get the artwork from LastFM then from iTunes if not found
                lastFmApi().fetchAlbumData(song.getAlbum(), false);

                if (song.getAlbum().getArtworkUrl().isEmpty()) {
                    artwork = GetArtworkFromiTunes.execute(
                        song
                    );
                }

                final var image = (null == song.getAlbum().getArtworkUrl() || song.getAlbum().getArtworkUrl().isEmpty())
                    ? new Image(albumThumbnailPlaceholderImageURL)
                    : new Image(song.getAlbum().getArtworkUrl());

                Platform.runLater(() -> {
                    // Set artwork thumbnail
                    albumArtwork.setImage(image);

                    final var imageColor = ImageHelper.calculateAverageColor(image);

                    // Set background color
                    if (settings().isBackgroundArtworkBind()) {
                        final var fadeTransition = new FadeTransition(Duration.seconds(0.5), playerContainer);
                        fadeTransition.setFromValue(0);
                        fadeTransition.setToValue(1);

                        fadeTransition.setOnFinished(event -> {
                            final var newBackground = new Background(new BackgroundFill(
                                imageColor.darker(),
                                CornerRadii.EMPTY,
                                null
                            ));
                            playerContainer.setBackground(newBackground);
                        });

                        fadeTransition.play();
                    } else {
                        playerContainer.setBackground(Background.EMPTY);
                    }

                    // Set shadow artwork
                    if (settings().isArtworkShadow()) {
                        albumArtworkShadow.setColor(imageColor.brighter());
                    }
                });

                return null;
            }
        };

        new Thread(getArtworkTask).start();
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

    @FXML public void trackTimeSliderClick(final MouseEvent event) {
        final var mouseX = event.getX();
        final var width = trackTimeSlider.getWidth();
        final var totalDuration = player().getTotalDuration().toMillis();
        final var seekTime = (mouseX / width) * totalDuration;

        player().seek(seekTime);
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
}
