package fr.xahla.musicx.desktop.views.content.localLibrary;

import fr.xahla.musicx.desktop.helper.*;
import fr.xahla.musicx.desktop.model.entity.Genre;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TableCell;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.image.ImageView;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.VBox;
import javafx.scene.text.Font;
import javafx.scene.text.FontPosture;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;
import javafx.util.Duration;

import java.net.URL;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.ResourceBundle;
import java.util.stream.Collectors;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for Songs group in Local Library scene.
 * @author Cochetooo
 * @since 0.3.3
 */
public class Songs implements Initializable {

    @FXML private TableView<Song> tracksTableView;

    @FXML private TableColumn<Song, LocalDateTime> tracksTableArtworkCol;
    @FXML private TableColumn<Song, LocalDateTime> tracksTableTitleCol;
    @FXML private TableColumn<Song, LocalDateTime> tracksTableGenresCol;
    @FXML private TableColumn<Song, LocalDate> tracksTableYearCol;
    @FXML private TableColumn<Song, Long> tracksTableDurationCol;

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        tracksTableArtworkCol.setCellValueFactory(cellData -> cellData.getValue().updatedAtProperty());
        tracksTableTitleCol.setCellValueFactory(cellData -> cellData.getValue().updatedAtProperty());
        tracksTableGenresCol.setCellValueFactory(cellData -> cellData.getValue().updatedAtProperty());

        tracksTableArtworkCol.setCellFactory(song -> new AlbumArtworkTableCell());
        tracksTableTitleCol.setCellFactory(song -> new TrackTitleTableCell());
        tracksTableGenresCol.setCellFactory(song -> new TrackGenresTableCell());
        tracksTableYearCol.setCellValueFactory(this::setAlbumYearTableCell);
        tracksTableDurationCol.setCellFactory(song -> new TrackDurationTableCell());

        scene().getLocalLibraryScene().resetFilters();
        this.addListeners();

        tracksTableView.setItems(scene().getLocalLibraryScene().getTrackList());
        tracksTableView.setTableMenuButtonVisible(false);
        tracksTableView.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY_ALL_COLUMNS);
    }

    // --- Initializers ---

    private void addListeners() {
        scene().onSearchTextChange((observable, oldValue, newValue) -> onSearch(newValue));
    }

    // --- Events ---

    private void onSearch(final String searchTerm) {
        scene().getLocalLibraryScene().setTrackListFilters(song ->
            song.getTitle().toLowerCase().contains(searchTerm.toLowerCase())
                || (null != song.getAlbum() && song.getAlbum().getName().toLowerCase().contains(searchTerm.toLowerCase()))
                || (null != song.getArtist() && song.getArtist().getName().toLowerCase().contains(searchTerm.toLowerCase()))
        );
    }

    @FXML private void onClick(final MouseEvent mouseEvent) {
        if (2 == mouseEvent.getClickCount()) {
            final var trackList = scene().getLocalLibraryScene().getTrackList();
            final var song = tracksTableView.getSelectionModel().getSelectedItem();
            final var index = trackList.indexOf(song);

            audioPlayer().setQueue(
                trackList,
                index
            );
        }
    }

    @FXML private void playNow() {
        audioPlayer().setCurrentSongByPosition(tracksTableView.getSelectionModel().getSelectedIndex());
    }

    @FXML private void queueNext() {
        audioPlayer().queueNext(tracksTableView.getSelectionModel().getSelectedItem());
    }

    @FXML private void queueLast() {
        audioPlayer().queueLast(tracksTableView.getSelectionModel().getSelectedItem());
    }

    @FXML private void editSong() {
        audioPlayer().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
        scene().getRightNavContent().switchContent(FxmlComponent.EDIT_SONG, this.resourceBundle);
    }

    @FXML private void editAlbum() {
        audioPlayer().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
        scene().getRightNavContent().switchContent(FxmlComponent.EDIT_ALBUM, this.resourceBundle);
    }

    @FXML private void editGenres() {
        audioPlayer().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
        scene().getRightNavContent().switchContent(FxmlComponent.EDIT_GENRE, this.resourceBundle);
    }

    // --- Cell Factories ---

    /* #######################
     * ###  Album Artwork  ###
     * ####################### */
    static class AlbumArtworkTableCell extends TableCell<Song, LocalDateTime> {
        @Override protected void updateItem(final LocalDateTime updatedAt, final boolean empty) {
            final var song = this.getTableRow().getItem();

            if (null == song || null == song.getAlbum() || StringHelper.str_is_null_or_blank(song.getAlbum().getArtworkUrl())) {
                setGraphic(null);
                return;
            }

            final var imageView = new ImageView();
            imageView.setImage(song.getAlbum().getImage());
            imageView.setFitWidth(36);
            imageView.setFitHeight(36);
            imageView.setPreserveRatio(true);
            ThemePolicyHelper.clipAlbumArtwork(imageView);

            setGraphic(imageView);
        }
    }

    /* #######################
     * ###   Track Title   ###
     * ####################### */
    static class TrackTitleTableCell extends TableCell<Song, LocalDateTime> {
        @Override protected void updateItem(final LocalDateTime updatedAt, final boolean empty) {
            final var song = this.getTableRow().getItem();

            // Song title must not be empty
            if (null == song || null == song.getTitle()) {
                setGraphic(null);
                return;
            }

            // Artist and album can be empty, if so it will just be an empty string
            final var artistName = (null == song.getArtist()) ? "" : song.getArtist().getName();
            final var albumName = (null == song.getAlbum()) ? "" : song.getAlbum().getName();

            final var vBox = TextHelper.caption(
                song.getTitle(),
                artistName + " - " + albumName,
                16,
                13,
                null,
                null
            );

            this.setGraphic(vBox);
        }
    }

    /* #######################
     * ###    Album Year   ###
     * ####################### */
    private ObjectProperty<LocalDate> setAlbumYearTableCell(final TableColumn.CellDataFeatures<Song, LocalDate> song) {
        if (null == song.getValue().getAlbum()) {
            return new SimpleObjectProperty<>(fr.xahla.musicx.domain.helper.DurationHelper.FIRST_DAY);
        }

        return song.getValue().getAlbum().releaseDateProperty();
    }

    /* #######################
     * ###   Track Genres  ###
     * ####################### */
    static class TrackGenresTableCell extends TableCell<Song, LocalDateTime> {
        @Override protected void updateItem(final LocalDateTime updatedAt, final boolean empty) {
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
            primaryGenresText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 15));
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
