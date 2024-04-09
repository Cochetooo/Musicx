package fr.xahla.musicx.desktop.views.content;

import atlantafx.base.controls.CustomTextField;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.logging.ErrorMessage;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TableCell;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableRow;
import javafx.scene.control.TableView;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.Background;
import javafx.scene.layout.Border;

import java.net.URL;
import java.time.Duration;
import java.util.ResourceBundle;
import java.util.function.Predicate;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;
import static fr.xahla.musicx.desktop.DesktopContext.*;

public class Content implements Initializable {

    private FilteredList<Song> filteredList;

    @FXML private TableView<Song> tracksTableView;

    @FXML private TableColumn<Song, String> tracksTableArtistCol;
    @FXML private TableColumn<Song, String> tracksTableAlbumCol;
    @FXML private TableColumn<Song, String> tracksTableTitleCol;
    @FXML private TableColumn<Song, Integer> tracksTableYearCol;
    @FXML private TableColumn<Song, Integer> tracksTableDurationCol;

    @FXML private CustomTextField searchTextField;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.filteredList = new FilteredList<>(trackList().getSongs());

        // Update search filters
        this.searchTextField.textProperty().addListener((observable, oldValue, newValue)
            -> this.filteredList.setPredicate(song ->
                song.getTitle().toLowerCase().contains(searchTextField.getText().toLowerCase())
                || song.getAlbum().getName().toLowerCase().contains(searchTextField.getText().toLowerCase())
                || song.getArtist().getName().toLowerCase().contains(searchTextField.getText().toLowerCase())
        ));

        this.tracksTableView.setItems(filteredList);

        this.tracksTableView.setRowFactory(row -> new TableRow<>(){

        });

        this.tracksTableArtistCol.setCellValueFactory(song -> song.getValue().getArtist().nameProperty());
        this.tracksTableAlbumCol.setCellValueFactory(song -> song.getValue().getAlbum().nameProperty());
        this.tracksTableTitleCol.setCellValueFactory(song -> song.getValue().titleProperty());
        this.tracksTableYearCol.setCellValueFactory(song -> song.getValue().getAlbum().releaseYearProperty().asObject());

        this.tracksTableDurationCol.setCellFactory(songDurationTableCol -> new TableCell<>(){
            @Override protected void updateItem(final Integer duration, final boolean empty) {
                if (empty || null == duration) {
                    setText(null);
                } else {
                    setText(DurationHelper.getDurationFormatted(Duration.ofSeconds(duration)));
                }
            }
        });
    }

    // --- Track Table ---

    @FXML public void tracksTableClick(final MouseEvent event) {
        if (2 == event.getClickCount()) {
            final var song = tracksTableView.getSelectionModel().getSelectedItem();
            final var index = trackList().getSongs().indexOf(song);

            if (!song.isAvailable()) {
                logger().info("Song not available: " + song.getFilePath());

                return;
            }

            if (-1 == index) {
                logger().warning(ErrorMessage.SONG_NOT_FOUND_IN_LIBRARY.getMsg(
                    tracksTableView.getSelectionModel().getSelectedItem().getFilePath()
                ));

                return;
            }

            player().setQueue(
                trackList().getSongs(),
                index
            );
        }
    }

    @FXML public void tracksTablePlayNow() {
        player().setCurrentSongByPosition(tracksTableView.getSelectionModel().getSelectedIndex());
    }

    @FXML public void tracksTableQueueNext() {
        player().queueNext(tracksTableView.getSelectionModel().getSelectedItem());
    }

    @FXML public void tracksTableQueueLast() {
        player().queueLast(tracksTableView.getSelectionModel().getSelectedItem());
    }
}
