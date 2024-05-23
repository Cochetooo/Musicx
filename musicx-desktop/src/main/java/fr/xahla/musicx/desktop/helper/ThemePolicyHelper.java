package fr.xahla.musicx.desktop.helper;

import javafx.scene.image.ImageView;
import javafx.scene.shape.Rectangle;

/**
 * Helper class for theme policy of the application,
 * such as paddings, margins, borders values, etc.
 * @author Cochetooo
 * @since 0.4.0
 */
public class ThemePolicyHelper {

    public static final int ALBUM_ARTWORK_CLIP_RADIUS = 10;

    public static final void clipAlbumArtwork(final ImageView artwork) {
        final var clip = new Rectangle(
            artwork.getFitWidth(),
            artwork.getFitHeight()
        );

        clip.setArcWidth(ALBUM_ARTWORK_CLIP_RADIUS);
        clip.setArcHeight(ALBUM_ARTWORK_CLIP_RADIUS);

        artwork.setClip(clip);
    }

}
