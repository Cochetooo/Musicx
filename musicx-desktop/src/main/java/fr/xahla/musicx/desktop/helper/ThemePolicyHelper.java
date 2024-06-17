package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.desktop.DesktopApplication;
import fr.xahla.musicx.domain.helper.FileHelper;
import javafx.scene.Scene;
import javafx.scene.image.ImageView;
import javafx.scene.shape.Rectangle;

import java.io.IOException;
import java.net.URISyntaxException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Objects;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Helper class for theme policy of the application,
 * such as paddings, margins, borders values, etc.
 * @author Cochetooo
 * @since 0.4.0
 */
public final class ThemePolicyHelper {

    public static final int ALBUM_ARTWORK_CLIP_RADIUS = 10;

    public static void loadAllStylesheets(final Scene scene) {
        try {
            Files.walk(Paths.get("musicx-desktop/src/main/resources/fr/xahla/musicx/desktop/assets/stylesheets"))
                .filter(Files::isRegularFile)
                .forEach(file -> scene.getStylesheets().add(file.toUri().toString()));
        } catch (final IOException exception) {
            logger().error(exception, "IO_FOLDER_BROWSE_ERROR", "assets/stylesheets");
        }
    }

    public static void clipAlbumArtwork(final ImageView artwork) {
        final var clip = new Rectangle(
            artwork.getFitWidth(),
            artwork.getFitHeight()
        );

        clip.setArcWidth(ALBUM_ARTWORK_CLIP_RADIUS);
        clip.setArcHeight(ALBUM_ARTWORK_CLIP_RADIUS);

        artwork.setClip(clip);
    }

}
