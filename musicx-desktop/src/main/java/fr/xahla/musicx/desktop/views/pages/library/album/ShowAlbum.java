package fr.xahla.musicx.desktop.views.pages.library.album;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.desktop.config.FxmlComponent;
import fr.xahla.musicx.desktop.factory.TrackDurationTableCell;
import fr.xahla.musicx.desktop.factory.TrackGenresTableCell;
import fr.xahla.musicx.desktop.factory.TrackNumberTableCell;
import fr.xahla.musicx.desktop.factory.TrackTitleTableCell;
import fr.xahla.musicx.desktop.helper.ImageHelper;
import fr.xahla.musicx.desktop.helper.ThemePolicyHelper;
import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.DurationHelper;
import javafx.collections.transformation.FilteredList;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Hyperlink;
import javafx.scene.control.Label;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.image.ImageView;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.Background;
import javafx.scene.layout.BackgroundFill;
import javafx.scene.layout.HBox;

import java.net.URL;
import java.time.Duration;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.application.AbstractContext.albumRepository;

/**
 * @author Cochetooo
 * @since 0.5.1
 */
public class ShowAlbum implements Initializable {

    @FXML private TableView<Song> tracksTableView;
    @FXML private TableColumn<Song, Integer> tracksTableNumberCol;
    @FXML private TableColumn<Song, Void> tracksTableTitleCol;
    @FXML private TableColumn<Song, Void> tracksTableGenresCol;
    @FXML private TableColumn<Song, Long> tracksTableDurationCol;

    @FXML private Label albumTypeLabel;
    @FXML private Label albumNameLabel;
    @FXML private Hyperlink artistLink;
    @FXML private Label yearDurationLabel;

    @FXML private HBox topContainer;

    @FXML private ImageView albumArtwork;

    private Album album;
    private FilteredList<Song> tracks;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        ThemePolicyHelper.clipAlbumArtwork(albumArtwork);

        album = scene().getLocalLibraryScene().getCurrentAlbum();
        tracks = new FilteredList<>(scene().getLocalLibraryScene().getTrackList());
        tracks.setPredicate(song -> song.getAlbum().equals(album));

        tracksTableNumberCol.setCellFactory(cell -> new TrackNumberTableCell());
        tracksTableTitleCol.setCellFactory(cell -> new TrackTitleTableCell());
        tracksTableGenresCol.setCellFactory(cell -> new TrackGenresTableCell());
        tracksTableDurationCol.setCellFactory(cell -> new TrackDurationTableCell());

        tracksTableView.setItems(tracks);
        tracksTableView.setColumnResizePolicy(TableView.CONSTRAINED_RESIZE_POLICY_ALL_COLUMNS);

        if (null != album.getType()) {
            albumTypeLabel.setText(album.getType().name());
        } else {
            albumTypeLabel.setText("Album");
        }

        albumNameLabel.setText(album.getName());

        if (null != album.getArtist()) {
            artistLink.setText(album.getArtist().getName());
        }

        if (null != album.getReleaseDate()) {
            final var totalDuration = albumRepository().getSongs(album.getDto()).stream()
                .mapToLong(SongDto::getDuration)
                .sum();

            yearDurationLabel.setText(
                album.getReleaseDate().toString() + " ◦ " + DurationHelper.getTimeString(totalDuration)
            );
        }

        final var imageColor = ImageHelper.calculateAverageColor(album.getImage()).darker();
        topContainer.setBackground(new Background(new BackgroundFill(imageColor, null, null)));

        albumArtwork.setImage(album.getImage());
    }

    @FXML private void backToTrackList() {
        scene().getSceneContent().switchContent(FxmlComponent.SCENE_LIBRARY);
    }

    @FXML private void playAlbum() {
        audioPlayer().setQueue(tracks, 0);
    }

    @FXML private void onClick(final MouseEvent mouseEvent) {
        if (2 == mouseEvent.getClickCount()
            && MouseButton.PRIMARY == mouseEvent.getButton()) {
            playNow();
        }
    }

    @FXML private void playNow() {
        final var song = tracksTableView.getSelectionModel().getSelectedItem();
        final var index = tracks.indexOf(song);

        audioPlayer().setQueue(
            tracks,
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
    }

    @FXML private void editAlbum() {
        audioPlayer().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
    }

    @FXML private void editGenres() {
        audioPlayer().setEditedSong(tracksTableView.getSelectionModel().getSelectedItem());
    }
}
