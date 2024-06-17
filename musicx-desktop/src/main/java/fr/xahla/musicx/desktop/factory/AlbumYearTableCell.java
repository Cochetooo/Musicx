package fr.xahla.musicx.desktop.factory;

import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.service.save.EditAlbumService;
import javafx.beans.property.SimpleObjectProperty;
import javafx.scene.control.DatePicker;
import javafx.scene.control.TableCell;
import javafx.scene.input.KeyCode;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;

import java.time.LocalDate;

/**
 * @author Cochetooo
 */
public class AlbumYearTableCell extends TableCell<Song, LocalDate> {

    private DatePicker releaseDatePicker;

    public AlbumYearTableCell() {
        this.setOnMouseClicked(this::handleMouseClick);
        this.setOnKeyPressed(event -> {
            if (null != releaseDatePicker) {
                if (KeyCode.ENTER == event.getCode()) {
                    commitEdit(releaseDatePicker.getValue());
                } else if (KeyCode.ESCAPE == event.getCode()) {
                    cancelEdit();
                }
            }
        });
    }

    private void handleMouseClick(final MouseEvent event) {
        if (event.getButton() == MouseButton.MIDDLE && 1 == event.getClickCount()) {
            startEdit();
        }
    }

    @Override public void startEdit() {
        super.startEdit();
        if (null == releaseDatePicker) {
            releaseDatePicker = new DatePicker(this.getItem());
        }

        setGraphic(releaseDatePicker);
        releaseDatePicker.requestFocus();
    }

    @Override public void cancelEdit() {
        super.cancelEdit();
        this.setGraphic(null);
    }

    @Override public void commitEdit(final LocalDate newValue) {
        super.commitEdit(null);

        final var album = this.getTableRow().getItem().getAlbum();

        if (null == album) {
            this.cancelEdit();
            return;
        }

        album.setReleaseDate(newValue);

        new EditAlbumService().execute(album, () -> this.setGraphic(null));
    }

    @Override protected void updateItem(final LocalDate item, final boolean empty) {
        final var song = this.getTableRow().getItem();

        if (null == song) {
            this.textProperty().unbind();
            setText("");
            return;
        }

        if (null == song.getAlbum()) {
            this.textProperty().bind(new SimpleObjectProperty<>(fr.xahla.musicx.domain.helper.DurationHelper.FIRST_DAY).asString());
        } else {
            this.textProperty().bind(song.getAlbum().releaseDateProperty().asString());
        }
    }

}
