package fr.xahla.musicx.desktop.views.pages.library.songs.table;

import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.config.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import fr.xahla.musicx.desktop.model.entity.Genre;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.collections.ListChangeListener;
import javafx.scene.Parent;
import javafx.scene.control.TableCell;
import javafx.scene.input.KeyCode;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.VBox;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;

import java.util.stream.Collectors;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;

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
    private Parent editContainer;

    public TrackGenresTableCell() {
        primaryGenresText.setFont(Font.font(FontTheme.FUTURISTIC_FONT.getFont(), FontWeight.BOLD, 14));
        primaryGenresText.setFill(ColorHelper.ALTERNATIVE);

        secondaryGenresText.setFont(Font.font(FontTheme.FUTURISTIC_FONT.getFont(), FontWeight.LIGHT, 11));
        secondaryGenresText.setFill(ColorHelper.TERNARY);

        setGraphic(vBox);

        this.setOnMouseClicked(this::handleMouseClick);

        this.setOnKeyPressed(event -> {
            if (null != editContainer) {
                if (KeyCode.ENTER == event.getCode()) {
                    commitEdit(null);
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

        audioPlayer().setEditedSong(currentSong);

        editContainer = FxmlHelper.getComponent(FxmlComponent.EDIT_GENRE.getFilepath());

        if (null == editContainer) {
            cancelEdit();
            return;
        }

        editContainer.setOpacity(0.9);

        setGraphic(editContainer);
        editContainer.requestFocus();
    }

    @Override public void cancelEdit() {
        super.cancelEdit();
        setGraphic(vBox);
        editContainer = null;
    }

    @Override public void commitEdit(final Void item) {
        super.commitEdit(item);
        setGraphic(vBox);
        editContainer = null;
    }

    @Override protected void updateItem(Void nothing, boolean empty) {
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
