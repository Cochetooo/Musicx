package fr.xahla.musicx.desktop.views.content;

import atlantafx.base.controls.CustomTextField;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.helper.FXMLHelper;
import fr.xahla.musicx.desktop.logging.ErrorMessage;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.collections.transformation.FilteredList;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.Parent;
import javafx.scene.control.TableCell;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.VBox;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;
import javafx.util.Duration;

import java.net.URL;
import java.time.LocalDate;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.*;

/** <b>View for the main center content with the track list and the search bar.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Content implements Initializable {

    private FilteredList<Song> filteredList;

    @FXML private TableView<Song> tracksTableView;

    @FXML private TableColumn<Song, String> tracksTableTitleCol;
    @FXML private TableColumn<Song, String> tracksTableGenresCol;
    @FXML private TableColumn<Song, LocalDate> tracksTableYearCol;
    @FXML private TableColumn<Song, Long> tracksTableDurationCol;

    @FXML private CustomTextField searchTextField;

    private ResourceBundle resourceBundle;

    // Right Nav Content
    private Parent queueListView;
    private Parent songEditView;
    private Parent albumEditView;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        // Right Nav Content
        this.queueListView = FXMLHelper.getComponent("content/queueList.fxml", resourceBundle);

        this.filteredList = new FilteredList<>(trackList().getSongs());

        // Update search filters
        this.searchTextField.textProperty().addListener((observable, oldValue, newValue)
            -> this.filteredList.setPredicate(song ->
                song.getTitle().toLowerCase().contains(searchTextField.getText().toLowerCase())
                || song.getAlbum().getName().toLowerCase().contains(searchTextField.getText().toLowerCase())
                || song.getArtist().getName().toLowerCase().contains(searchTextField.getText().toLowerCase())
        ));

        this.tracksTableView.setItems(filteredList);
        this.tracksTableView.setTableMenuButtonVisible(false);
        this.tracksTableView.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY_ALL_COLUMNS);

        this.tracksTableYearCol.setCellValueFactory(song -> {
            if (null == song.getValue().getAlbum()) {
                return new SimpleObjectProperty<>(LocalDate.MIN);
            }

            return song.getValue().getAlbum().releaseDateProperty();
        });

        this.tracksTableTitleCol.setCellFactory(songStringTableColumn -> new TableCell<>() {
            @Override protected void updateItem(final String value, final boolean empty) {
                super.updateItem(value, empty);

                final var song = this.getTableRow().getItem();

                if (null == song || empty) {
                    setText(null);
                    setGraphic(null);
                } else {
                    final var titleText = new Text(song.getTitle());
                    titleText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 15));
                    titleText.setFill(ColorHelper.PRIMARY);

                    final var artistName = (null == song.getArtist()) ? "" : song.getArtist().getName();
                    final var albumName = (null == song.getAlbum()) ? "" : song.getAlbum().getName();

                    final var artistText = new Text(artistName + " - " + albumName);
                    artistText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
                    artistText.setFill(ColorHelper.GRAY);

                    final var vBox = new VBox(titleText, artistText);
                    this.setGraphic(vBox);
                }
            }
        });

        this.tracksTableGenresCol.setCellFactory(songStringTableColumn -> new TableCell<>() {
            @Override protected void updateItem(final String value, final boolean empty) {
                super.updateItem(value, empty);

                final var song = this.getTableRow().getItem();

                if (null == song || empty) {
                    setText(null);
                    setGraphic(null);
                } else {
                    final String[] genresText = {"", ""};
                    song.getPrimaryGenres().forEach(primaryGenre -> {
                        genresText[0] += primaryGenre.getName() + ", ";
                    });

                    if (genresText[0].length() > 2) {
                        genresText[0] = genresText[0].substring(0, genresText[0].length()-2);
                    }

                    final var primaryGenres = new Text(genresText[0]);
                    primaryGenres.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 13));
                    primaryGenres.setFill(ColorHelper.ALTERNATIVE);

                    song.getSecondaryGenres().forEach(secondaryGenre -> {
                        genresText[1] += secondaryGenre.getName() + ", ";
                    });

                    if (genresText[1].length() > 2) {
                        genresText[1] = genresText[1].substring(0, genresText[1].length()-2);
                    }

                    final var secondaryGenres = new Text(genresText[1]);
                    secondaryGenres.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
                    secondaryGenres.setFill(ColorHelper.TERNARY);

                    final var vBox = new VBox(primaryGenres, secondaryGenres);
                    this.setGraphic(vBox);
                }
            }
        });

        this.tracksTableDurationCol.setCellFactory(songDurationTableCol -> new TableCell<>(){
            @Override protected void updateItem(final Long duration, final boolean empty) {
                if (empty || null == duration) {
                    setText(null);
                } else {
                    setText(DurationHelper.getTimeString(Duration.millis(duration)));
                }
            }
        });
    }

    // --- Track Table ---

    @FXML public void tracksTableClick(final MouseEvent event) {
        if (2 == event.getClickCount()) {
            final var song = tracksTableView.getSelectionModel().getSelectedItem();
            final var index = trackList().getSongs().indexOf(song);

            if (-1 == index) {
                logger().warning(ErrorMessage.SONG_NOT_FOUND_IN_LIBRARY.getMsg(
                    tracksTableView.getSelectionModel().getSelectedItem().getFilepath()
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

    @FXML public void actionSettings() {
        FXMLHelper.showModal("settings.fxml", this.resourceBundle, resourceBundle.getString("settings.title"));
    }

    @FXML public void actionQueueList() {
        if (null != rightNavContent().get() && rightNavContent().get().equals(queueListView)) {
            rightNavContent().set(null);
            return;
        }

        rightNavContent().set(queueListView);
    }

    @FXML public void actionSongEdit() {
        if (null != rightNavContent().get() && rightNavContent().get().equals(songEditView)) {
            rightNavContent().set(null);
            return;
        }

        this.songEditView = FXMLHelper.getComponent("content/edit/songEdit.fxml", resourceBundle);

        rightNavContent().set(null);
        rightNavContent().set(songEditView);
    }

    @FXML public void actionAlbumEdit() {
        if (null != rightNavContent().get() && rightNavContent().get().equals(albumEditView)) {
            rightNavContent().set(null);
            return;
        }

        this.albumEditView = FXMLHelper.getComponent("content/edit/albumEdit.fxml", resourceBundle);

        rightNavContent().set(null);
        rightNavContent().set(albumEditView);
    }
}
