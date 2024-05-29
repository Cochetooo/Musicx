package fr.xahla.musicx.desktop.model.table;

import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.scene.control.Label;
import javafx.scene.control.TableCell;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;

/**
 * @author Cochetooo
 * @since 0.4.1
 */
public class TrackTitleTableCell extends TableCell<Song, Void> {
    private final VBox container;
    private final Label songText = new Label();
    private final Label artistText = new Label();
    private final Label albumText = new Label();

    public TrackTitleTableCell() {
        songText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 15));
        songText.setTextFill(ColorHelper.PRIMARY);

        final var secondLine = new HBox();
        secondLine.setSpacing(5);
        final var space = new Text("-");

        space.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
        space.setFill(ColorHelper.SECONDARY);

        artistText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
        artistText.setTextFill(ColorHelper.SECONDARY);

        albumText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
        albumText.setTextFill(ColorHelper.SECONDARY);

        secondLine.getChildren().addAll(artistText, new Text("-"), albumText);

        container = new VBox();
        container.getChildren().addAll(songText, secondLine);
    }

    @Override protected void updateItem(final Void nothing, final boolean empty) {
        final var song = this.getTableRow().getItem();

        // Song title must not be empty
        if (null == song || null == song.getTitle()) {
            setGraphic(null);
            return;
        }

        this.update();
    }

    private void update() {
        final var song = this.getTableRow().getItem();

        final var artist = song.getArtist();
        final var album = song.getAlbum();

        songText.textProperty().unbind();
        artistText.textProperty().unbind();
        albumText.textProperty().unbind();

        songText.textProperty().bind(song.titleProperty());
        if (artist != null) {
            artistText.textProperty().bind(artist.nameProperty());
        } else {
            artistText.setText("");
        }
        if (album != null) {
            albumText.textProperty().bind(album.nameProperty());
        } else {
            albumText.setText("");
        }

        this.setGraphic(container);
    }
}