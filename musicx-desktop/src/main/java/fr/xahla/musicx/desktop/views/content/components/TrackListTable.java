package fr.xahla.musicx.desktop.views.content.components;

import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.desktop.model.entity.Genre;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TableCell;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.image.ImageView;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.VBox;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;
import javafx.util.Duration;

import java.net.URL;
import java.time.LocalDate;
import java.util.ResourceBundle;
import java.util.stream.Collectors;

import static fr.xahla.musicx.desktop.context.DesktopContext.*;

/**
 * Defines the table displaying songs list
 * @author Cochetooo
 * @since 0.3.3
 */
public class TrackListTable implements Initializable {

    private FilteredList<Song> filteredList;

    @FXML private TableView<Song> tracksTableView;

    @FXML private TableColumn<Song, Void> tracksTableArtworkCol;
    @FXML private TableColumn<Song, Void> tracksTableTitleCol;
    @FXML private TableColumn<Song, Void> tracksTableGenresCol;
    @FXML private TableColumn<Song, LocalDate> tracksTableYearCol;
    @FXML private TableColumn<Song, Long> tracksTableDurationCol;

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
        this.filteredList = new FilteredList<>(trackList().getSongs());

        tracksTableArtworkCol.setCellFactory(song -> new AlbumArtworkTableCell());
        tracksTableTitleCol.setCellFactory(song -> new TrackTitleTableCell());
        tracksTableGenresCol.setCellFactory(song -> new TrackGenresTableCell());
        tracksTableYearCol.setCellValueFactory(this::setAlbumYearTableCell);
        tracksTableDurationCol.setCellFactory(song -> new TrackDurationTableCell());

        this.addListeners();

        tracksTableView.setItems(filteredList);
        tracksTableView.setTableMenuButtonVisible(false);
        tracksTableView.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY_ALL_COLUMNS);
    }

    // --- Initializers ---

    private void addListeners() {
       controller().searchTextProperty().addListener((observable, oldValue, newValue) -> onSearch(newValue));
    }

    // --- Events ---

    private void onSearch(final String searchTerm) {
        this.filteredList.setPredicate(song ->
            song.getTitle().toLowerCase().contains(searchTerm.toLowerCase())
                || (null != song.getAlbum() && song.getAlbum().getName().toLowerCase().contains(searchTerm.toLowerCase()))
                || (null != song.getArtist() && song.getArtist().getName().toLowerCase().contains(searchTerm.toLowerCase()))
        );
    }

    @FXML private void onClick(final MouseEvent mouseEvent) {
        if (2 == mouseEvent.getClickCount()) {
            final var song = tracksTableView.getSelectionModel().getSelectedItem();
            final var index = trackList().getSongs().indexOf(song);

            player().setQueue(
                trackList().getSongs(),
                index
            );
        }
    }

    @FXML private void playNow() {
        player().setCurrentSongByPosition(tracksTableView.getSelectionModel().getSelectedIndex());
    }

    @FXML private void queueNext() {
        player().queueNext(tracksTableView.getSelectionModel().getSelectedItem());
    }

    @FXML private void queueLast() {
        player().queueLast(tracksTableView.getSelectionModel().getSelectedItem());
    }

    @FXML private void editSong() {
        player().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
        rightNavContent().switchContent(FxmlComponent.EDIT_SONG, this.resourceBundle);
    }

    @FXML private void editAlbum() {
        player().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
        rightNavContent().switchContent(FxmlComponent.EDIT_ALBUM, this.resourceBundle);
    }

    // --- Cell Factories ---

    /* #######################
     * ###  Album Artwork  ###
     * ####################### */
    static class AlbumArtworkTableCell extends TableCell<Song, Void> {
        @Override protected void updateItem(final Void nothing, final boolean empty) {
            final var song = this.getTableRow().getItem();

            if (null == song || null == song.getAlbum() || StringHelper.str_is_null_or_blank(song.getAlbum().getArtworkUrl())) {
                setGraphic(null);
                return;
            }

            final var imageView = new ImageView();
            imageView.setImage(song.getAlbum().getImage());
            imageView.setFitWidth(32);
            imageView.setFitHeight(32);
            imageView.setPreserveRatio(true);
            setGraphic(imageView);
        }
    }

    /* #######################
     * ###   Track Title   ###
     * ####################### */
    static class TrackTitleTableCell extends TableCell<Song, Void> {
        @Override protected void updateItem(final Void nothing, final boolean empty) {
            final var song = this.getTableRow().getItem();

            // Song title must not be empty
            if (null == song || null == song.getTitle()) {
                setGraphic(null);
                return;
            }

            // Set song title
            final var titleText = new Text(song.getTitle());
            titleText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 15));
            titleText.setFill(ColorHelper.PRIMARY);

            // Artist and album can be empty, if so it will just be an empty string
            final var artistName = (null == song.getArtist()) ? "" : song.getArtist().getName();
            final var albumName = (null == song.getAlbum()) ? "" : song.getAlbum().getName();

            final var artistText = new Text(artistName + " - " + albumName);
            artistText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
            artistText.setFill(ColorHelper.GRAY);

            final var vBox = new VBox(titleText, artistText);
            this.setGraphic(vBox);
        }
    }

    /* #######################
     * ###    Album Year   ###
     * ####################### */
    private ObjectProperty<LocalDate> setAlbumYearTableCell(final TableColumn.CellDataFeatures<Song, LocalDate> song) {
        if (null == song.getValue().getAlbum()) {
            return new SimpleObjectProperty<>(LocalDate.MIN);
        }

        return song.getValue().getAlbum().releaseDateProperty();
    }

    /* #######################
     * ###   Track Genres  ###
     * ####################### */
    static class TrackGenresTableCell extends TableCell<Song, Void> {
        @Override protected void updateItem(final Void nothing, final boolean empty) {
            final var song = this.getTableRow().getItem();

            if (null == song || (song.getPrimaryGenres().isEmpty() && song.getSecondaryGenres().isEmpty())) {
                setGraphic(null);
                return;
            }

            final var genresText = new String[]{
                song.getPrimaryGenres().stream()
                    .map(Genre::getName)
                    .collect(Collectors.joining(", ")),
                song.getSecondaryGenres().stream()
                    .map(Genre::getName)
                    .collect(Collectors.joining(", "))
            };

            final var primaryGenresText = new Text(genresText[0]);
            primaryGenresText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 13));
            primaryGenresText.setFill(ColorHelper.ALTERNATIVE);

            final var secondaryGenresText = new Text(genresText[1]);
            secondaryGenresText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
            secondaryGenresText.setFill(ColorHelper.TERNARY);

            final var vBox = new VBox(primaryGenresText, secondaryGenresText);
            this.setGraphic(vBox);
        }
    }

    /* #######################
     * ###  Track Duration ###
     * ####################### */
    static class TrackDurationTableCell extends TableCell<Song, Long> {
        @Override protected void updateItem(final Long duration, final boolean empty) {
            if (null == duration || empty) {
                setText(null);
            } else {
                setText(DurationHelper.getTimeString(Duration.millis(duration)));
            }
        }
    }
}
