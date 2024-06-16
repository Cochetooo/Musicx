package fr.xahla.musicx.desktop.views.pages.library.songs;

import atlantafx.base.controls.CustomTextField;
import fr.xahla.musicx.desktop.model.entity.Genre;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.views.pages.library.songs.table.*;
import javafx.animation.FadeTransition;
import javafx.animation.SequentialTransition;
import javafx.animation.TranslateTransition;
import javafx.application.Platform;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.concurrent.Task;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.geometry.Pos;
import javafx.scene.control.*;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.StackPane;
import javafx.util.Duration;
import org.controlsfx.control.CheckComboBox;

import java.net.URL;
import java.time.LocalDate;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.helper.ArrayHelper.array_contains_string_ignore_case;

/**
 * View for Songs group in Local Library scene.
 * @author Cochetooo
 * @since 0.3.3
 */
public class LibrarySongList implements Initializable {

    @FXML private CustomTextField searchTextField;

    @FXML private TableView<Song> tracksTableView;

    @FXML private TableColumn<Song, Void> tracksTableArtworkCol;
    @FXML private TableColumn<Song, Void> tracksTableTitleCol;
    @FXML private TableColumn<Song, Void> tracksTableGenresCol;
    @FXML private TableColumn<Song, LocalDate> tracksTableYearCol;
    @FXML private TableColumn<Song, Long> tracksTableDurationCol;

    @FXML private CheckComboBox<FilterSettings> filterSettingsComboBox;

    private Thread filterListThread;
    @FXML private ProgressIndicator filterProgressIndicator;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        searchTextField.textProperty().addListener(((observable, oldValue, newValue) -> this.onSearch(newValue)));

        tracksTableArtworkCol.setCellFactory(song -> new AlbumArtworkTableCell());
        tracksTableTitleCol.setCellFactory(song -> new TrackTitleTableCell());
        tracksTableGenresCol.setCellFactory(song -> new TrackGenresTableCell());
        tracksTableYearCol.setCellFactory(song -> new AlbumYearTableCell());
        tracksTableDurationCol.setCellFactory(song -> new TrackDurationTableCell());

        tracksTableYearCol.setCellValueFactory(this::setAlbumYearTableCell);

        filterSettingsComboBox.getItems().addAll(FilterSettings.values());
        filterSettingsComboBox.getCheckModel().checkAll();

        scene().getLocalLibraryScene().resetFilters();
        this.addListeners();

        tracksTableView.setItems(scene().getLocalLibraryScene().getTrackList());
        tracksTableView.setTableMenuButtonVisible(false);
        tracksTableView.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY_ALL_COLUMNS);

        tracksTableView.addEventFilter(MouseEvent.MOUSE_PRESSED, event -> {
            if (event.getTarget() instanceof final TableCell<?, ?> tableCell) {
                if (tableCell.isEditing()) {
                    return;
                }
            }

            tracksTableView.edit(-1, null);
        });

        StackPane.setAlignment(filterProgressIndicator, Pos.CENTER);
    }

    // --- Initializers ---

    private void addListeners() {
        scene().onSearchTextChange((observable, oldValue, newValue) -> onSearch(newValue));
    }

    // --- Events ---

    private void onSearch(final String searchTerm) {
        filterProgressIndicator.setVisible(true);

        if (null != filterListThread) {
            filterListThread.interrupt();
        }

        final var filterListTask = new Task<>() {
            @Override protected Void call() {
                scene().getLocalLibraryScene().setTrackListFilters(song -> {
                    boolean filtering = false;

                    for (final var setting : filterSettingsComboBox.getCheckModel().getCheckedItems()) {
                        switch (setting) {
                            case SONG_TITLE         -> filtering |= song.getTitle().toLowerCase().contains(searchTerm.toLowerCase());
                            case ALBUM_NAME         -> filtering |= (null != song.getAlbum() && song.getAlbum().getName().toLowerCase().contains(searchTerm.toLowerCase()));
                            case ARTIST_NAME        -> filtering |= (null != song.getArtist() && song.getArtist().getName().toLowerCase().contains(searchTerm.toLowerCase()));
                            case PRIMARY_GENRE      -> filtering |= array_contains_string_ignore_case(searchTerm, song.getPrimaryGenres().stream().map(Genre::getName).toList());
                            case SECONDARY_GENRE    -> filtering |= array_contains_string_ignore_case(searchTerm, song.getSecondaryGenres().stream().map(Genre::getName).toList());
                            case RELEASE_DATE       -> filtering |= (null != song.getAlbum() && String.valueOf(song.getAlbum().getReleaseDate().getYear()).contains(searchTerm));
                        }
                    }

                    return filtering;
                });

                return null;
            }
        };

        filterListTask.setOnSucceeded(event -> {
            filterProgressIndicator.setVisible(false);

            for (final var song : tracksTableView.getItems()) {
                Platform.runLater(() -> {
                    final var row = new TableRow<Song>();
                    row.setItem(song);
                    final var animation = createRowAnimation(row);
                    animation.play();
                });
            }
        });

        filterListThread = new Thread(filterListTask);
        filterListThread.start();
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

    // --- Animations ---

    public SequentialTransition createRowAnimation(final TableRow<Song> row) {
        final var translate = new TranslateTransition(Duration.millis(200), row);
        translate.setFromX(tracksTableView.getWidth());
        translate.setToX(0);

        final var fade = new FadeTransition(Duration.millis(200), row);
        fade.setFromValue(0);
        fade.setToValue(1);

        return new SequentialTransition(row, translate, fade);
    }

    // --- Enums ---

    enum FilterSettings {
        SONG_TITLE,
        ALBUM_NAME,
        ARTIST_NAME,
        PRIMARY_GENRE,
        SECONDARY_GENRE,
        RELEASE_DATE
    }

}
