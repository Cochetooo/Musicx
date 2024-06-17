package fr.xahla.musicx.desktop.factory;

import fr.xahla.musicx.desktop.helper.ThemePolicyHelper;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.StringHelper;
import javafx.scene.control.TableCell;
import javafx.scene.image.ImageView;

/**
 * @author Cochetooo
 * @since 0.4.1
 */
public class AlbumArtworkTableCell extends TableCell<Song, Void> {
    private final ImageView imageView;

    public AlbumArtworkTableCell() {
        imageView = new ImageView();
        imageView.setFitWidth(36);
        imageView.setFitHeight(36);
        imageView.setPreserveRatio(true);
        ThemePolicyHelper.clipAlbumArtwork(imageView);
    }

    @Override protected void updateItem(final Void nothing, final boolean empty) {
        final var song = this.getTableRow().getItem();

        if (null == song || null == song.getAlbum() || StringHelper.str_is_null_or_blank(song.getAlbum().getArtworkUrl())) {
            setGraphic(null);
            return;
        }

        imageView.imageProperty().unbind();
        imageView.imageProperty().bind(song.getAlbum().imageProperty());

        setGraphic(imageView);
    }
}
