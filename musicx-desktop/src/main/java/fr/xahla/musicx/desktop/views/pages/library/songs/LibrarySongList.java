package fr.xahla.musicx.desktop.views.pages.library.songs;

import fr.xahla.musicx.desktop.helper.*;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.views.pages.library.songs.table.AlbumArtworkTableCell;
import fr.xahla.musicx.desktop.views.pages.library.songs.table.TrackDurationTableCell;
import fr.xahla.musicx.desktop.views.pages.library.songs.table.TrackGenresTableCell;
import fr.xahla.musicx.desktop.views.pages.library.songs.table.TrackTitleTableCell;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TableCell;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;
import javafx.util.Duration;

import java.net.URL;
import java.time.LocalDate;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for Songs group in Local Library scene.
 * @author Cochetooo
 * @since 0.3.3
 */
public class LibrarySongList implements Initializable {

    @FXML private TableView<Song> tracksTableView;

    @FXML private TableColumn<Song, Void> tracksTableArtworkCol;
    @FXML private TableColumn<Song, Void> tracksTableTitleCol;
    @FXML private TableColumn<Song, Void> tracksTableGenresCol;
    @FXML private TableColumn<Song, LocalDate> tracksTableYearCol;
    @FXML private TableColumn<Song, Long> tracksTableDurationCol;

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        tracksTableYearCol.setCellValueFactory(this::setAlbumYearTableCell);

        tracksTableArtworkCol.setCellFactory(song -> new AlbumArtworkTableCell());
        tracksTableTitleCol.setCellFactory(song -> new TrackTitleTableCell());
        tracksTableGenresCol.setCellFactory(song -> new TrackGenresTableCell());
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
        if (2 == mouseEvent.getClickCount()
                && MouseButton.PRIMARY == mouseEvent.getButton()) {
            playNow();
        }
    }

    @FXML private void playNow() {
        final var trackList = scene().getLocalLibraryScene().getTrackList();
        final var song = tracksTableView.getSelectionModel().getSelectedItem();
        final var index = trackList.indexOf(song);

        audioPlayer().setQueue(
            trackList,
            index
        );
    }

    @FXML private void queueNext() {
        audioPlayer().queueNext(tracksTableView.getSelectionModel().getSelectedItem());
    }

    @FXML private void queueLast() {
        audioPlayer().queueLast(tracksTableView.getSelectionModel().getSelectedItem());
    }

    @FXML private void editSong() {
        audioPlayer().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
        //scene().getRightNavContent().switchContent(FxmlComponent.EDIT_SONG, this.resourceBundle);
    }

    @FXML private void editAlbum() {
        audioPlayer().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
        //scene().getRightNavContent().switchContent(FxmlComponent.EDIT_ALBUM, this.resourceBundle);
    }

    @FXML private void editGenres() {
        audioPlayer().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
        //scene().getRightNavContent().switchContent(FxmlComponent.EDIT_GENRE, this.resourceBundle);
    }

    // --- Cell Factories ---

    /* #######################
     * ###    Album Year   ###
     * ####################### */
    private ObjectProperty<LocalDate> setAlbumYearTableCell(final TableColumn.CellDataFeatures<Song, LocalDate> song) {
        if (null == song.getValue().getAlbum()) {
            return new SimpleObjectProperty<>(fr.xahla.musicx.domain.helper.DurationHelper.FIRST_DAY);
        }

        return song.getValue().getAlbum().releaseDateProperty();
    }

}
