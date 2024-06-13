package fr.xahla.musicx.desktop.views.pages.visualizer;

import fr.xahla.musicx.desktop.helper.ImageHelper;
import fr.xahla.musicx.desktop.helper.animation.ColorTransition;
import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Label;
import javafx.scene.effect.DropShadow;
import javafx.scene.image.ImageView;
import javafx.scene.layout.Background;
import javafx.scene.layout.BackgroundFill;
import javafx.scene.layout.CornerRadii;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.scene.text.Font;
import javafx.util.Duration;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class NowPlaying implements Initializable {

    @FXML private VBox nowPlayingContainer;

    @FXML private ImageView albumArtwork;
    @FXML private DropShadow albumArtworkShadow;

    @FXML private Label trackNameLabel;
    @FXML private Label artistAlbumLabel;

    @FXML private Label primaryGenresText;
    @FXML private Label secondaryGenresText;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        nowPlayingContainer.setBackground(Background.fill(Color.TRANSPARENT));

        this.onSongChange(audioPlayer().getCurrentSong());
        audioPlayer().onSongChange(this::onSongChange);

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

        final var currentBackgroundColor = (Color) nowPlayingContainer.getBackground().getFills().getFirst().getFill();
        final var imageColor = ImageHelper.calculateAverageColor(image).darker().darker();

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
            albumArtworkShadow.setColor(imageColor.brighter());
        }
    }

}
