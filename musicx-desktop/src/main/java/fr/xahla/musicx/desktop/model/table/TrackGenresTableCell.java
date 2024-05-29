package fr.xahla.musicx.desktop.model.table;

import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.model.entity.Genre;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.collections.ListChangeListener;
import javafx.scene.control.TableCell;
import javafx.scene.layout.VBox;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;

import java.util.stream.Collectors;

/**
 * @author Cochetooo
 * @since 0.4.1
 */
public class TrackGenresTableCell extends TableCell<Song, Void> {
    private final Text primaryGenresText = new Text();
    private final Text secondaryGenresText = new Text();
    private final VBox vBox = new VBox(primaryGenresText, secondaryGenresText);

    private ListChangeListener<Genre> primaryGenresListener;
    private ListChangeListener<Genre> secondaryGenresListener;

    private Song currentSong;

    public TrackGenresTableCell() {
        primaryGenresText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 15));
        primaryGenresText.setFill(ColorHelper.ALTERNATIVE);

        secondaryGenresText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
        secondaryGenresText.setFill(ColorHelper.TERNARY);

        setGraphic(vBox);
    }

    @Override
    protected void updateItem(Void nothing, boolean empty) {
        super.updateItem(nothing, empty);

        if (empty || getTableRow() == null || getTableRow().getItem() == null) {
            setGraphic(null);
            this.removeListeners();
            currentSong = null;
            return;
        }

        final var song = getTableRow().getItem();

        if (song == null) {
            setGraphic(null);
            this.removeListeners();
            currentSong = null;
            return;
        }

        if (currentSong == song) {
            return;
        }

        this.removeListeners();
        currentSong = song;

        primaryGenresListener = change -> updateGenresText(song);
        secondaryGenresListener = change -> updateGenresText(song);

        song.primaryGenresProperty().addListener(primaryGenresListener);
        song.secondaryGenresProperty().addListener(secondaryGenresListener);

        updateGenresText(song);
    }

    private void updateGenresText(Song song) {
        String primaryGenres = song.getPrimaryGenres().stream()
            .map(Genre::getName)
            .collect(Collectors.joining(", "));
        String secondaryGenres = song.getSecondaryGenres().stream()
            .map(Genre::getName)
            .collect(Collectors.joining(", "));

        primaryGenresText.setText(primaryGenres);
        secondaryGenresText.setText(secondaryGenres);

        setGraphic(vBox);
    }

    private void removeListeners() {
        if (currentSong != null) {
            currentSong.primaryGenresProperty().removeListener(primaryGenresListener);
            currentSong.secondaryGenresProperty().removeListener(secondaryGenresListener);
        }
    }
}
