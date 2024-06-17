package fr.xahla.musicx.desktop.views.pages.visualizer;

import fr.xahla.musicx.desktop.helper.ImageHelper;
import fr.xahla.musicx.desktop.helper.animation.ColorTransition;
import fr.xahla.musicx.desktop.model.entity.Genre;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Label;
import javafx.scene.effect.DropShadow;
import javafx.scene.image.ImageView;
import javafx.scene.image.WritableImage;
import javafx.scene.layout.*;
import javafx.scene.paint.Color;
import javafx.scene.paint.CycleMethod;
import javafx.scene.paint.LinearGradient;
import javafx.scene.paint.Stop;
import javafx.scene.shape.Rectangle;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.util.Duration;

import java.net.URL;
import java.util.ResourceBundle;
import java.util.stream.Collectors;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class NowPlaying implements Initializable {

    @FXML private Double PARALLAX_FACTOR;
    @FXML private Double IMAGE_SIZE;
    @FXML private Double CROP_REFLECTION_HEIGHT;

    @FXML private HBox nowPlayingContainer;

    @FXML private ImageView albumArtwork;
    @FXML private DropShadow albumArtworkShadow;

    @FXML private ImageView albumArtworkReflection;
    @FXML private DropShadow albumArtworkReflectionShadow;

    @FXML private Label trackNameLabel;
    @FXML private Label artistAlbumLabel;

    @FXML private Label primaryGenresText;
    @FXML private Label secondaryGenresText;
    @FXML private Label bitRateText;
    @FXML private Label formatText;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        nowPlayingContainer.setBackground(Background.fill(Color.TRANSPARENT));

        this.onSongChange(audioPlayer().getCurrentSong());
        audioPlayer().onSongChange(this::onSongChange);

        final var clip = getReflectionClip();
        albumArtworkReflection.setClip(clip);

        trackNameLabel.setFont(Font.font(24));
        artistAlbumLabel.setFont(Font.font(18));
    }

    private void onSongChange(final Song song) {
        trackNameLabel.setText(song.getTitle());

        final var artistAlbumText = new StringBuilder();

        if (null != song.getArtist()) {
            artistAlbumText.append(song.getArtist().getName()).append(" - ");
        }

        if (null != song.getAlbum()) {
            artistAlbumText.append(song.getAlbum().getName());
        }

        artistAlbumLabel.setText(artistAlbumText.toString());

        final var image = song.getAlbum().getImage();

        albumArtwork.setImage(image);

        final var virtualHeight = (int) (image.getHeight() / (IMAGE_SIZE / CROP_REFLECTION_HEIGHT));
        final var croppedImage = new WritableImage(
            image.getPixelReader(),
            0, virtualHeight * (int) (IMAGE_SIZE / CROP_REFLECTION_HEIGHT - 1),
            (int) image.getWidth(),
            (int) image.getHeight() / (int) (IMAGE_SIZE / CROP_REFLECTION_HEIGHT)
        );

        albumArtworkReflection.setImage(croppedImage);

        final var currentBackgroundColor = (Color) nowPlayingContainer.getBackground().getFills().getFirst().getFill();
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
                    nowPlayingContainer.setBackground(newBackground);
                }
            ).play();
        } else {
            nowPlayingContainer.setBackground(Background.fill(Color.TRANSPARENT));
        }

        // Set shadow artwork
        if (scene().getSettings().isArtworkShadow()) {
            final var shadowColor = imageColor.brighter();

            albumArtworkShadow.setColor(shadowColor);
            albumArtworkReflectionShadow.setColor(shadowColor);
        }

        primaryGenresText.setText(
            song.getPrimaryGenres().stream()
                .map(Genre::getName)
                .collect(Collectors.joining(", "))
        );

        secondaryGenresText.setText(
            song.getSecondaryGenres().stream()
                .map(Genre::getName)
                .collect(Collectors.joining(", "))
        );

        bitRateText.setText(song.getBitRate() + " Kbps - " + song.getSampleRate() + " Hz");
        formatText.setText("Audio Format: " + song.getFormat().name());
    }

    private Rectangle getReflectionClip() {
        final var clip = new Rectangle(IMAGE_SIZE, IMAGE_SIZE);
        final var gradient = new LinearGradient(
            0, 0, 0, 1,
            true,
            CycleMethod.NO_CYCLE,
            new Stop(0, Color.TRANSPARENT),
            new Stop(CROP_REFLECTION_HEIGHT / IMAGE_SIZE, Color.color(1, 1, 1, 0.45))
        );

        clip.setFill(gradient);
        return clip;
    }

}
