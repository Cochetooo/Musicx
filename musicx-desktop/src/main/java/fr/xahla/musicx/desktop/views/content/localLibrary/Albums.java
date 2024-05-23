package fr.xahla.musicx.desktop.views.content.localLibrary;

import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.helper.TextHelper;
import fr.xahla.musicx.desktop.helper.ThemePolicyHelper;
import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.control.Label;
import javafx.scene.control.ListView;
import javafx.scene.image.ImageView;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.HBox;
import javafx.scene.layout.StackPane;
import javafx.scene.layout.TilePane;
import javafx.scene.layout.VBox;
import javafx.scene.shape.Rectangle;
import javafx.scene.text.*;
import javafx.util.Duration;

import java.net.URL;
import java.util.ArrayList;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.application.AbstractContext.*;

/**
 * View for Albums group in Local Library scene.
 * @author Cochetooo
 * @since 0.4.0
 */
public class Albums implements Initializable {

    @FXML private TilePane albumList;

    @FXML private StackPane selectedAlbumContainer;
    @FXML private ImageView selectedAlbumImageView;
    @FXML private Label selectedAlbumLabel;
    @FXML private Label selectedAlbumArtistLabel;
    @FXML private TilePane selectedAlbumTrackList;

    private ListProperty<Album> albums;
    private VBox selectedAlbum;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var albumsDto = albumRepository().findAll();

        selectedAlbumContainer.setPrefHeight(albumList.getHeight() / 2);

        ThemePolicyHelper.clipAlbumArtwork(selectedAlbumImageView);

        selectedAlbumLabel.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 14));
        selectedAlbumLabel.setTextFill(ColorHelper.PRIMARY);

        selectedAlbumArtistLabel.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
        selectedAlbumArtistLabel.setTextFill(ColorHelper.SECONDARY);

        albums = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        albumsDto.forEach((albumDto) -> albums.add(new Album(albumDto)));

        albums.forEach(this::addAlbumTile);
    }

    private void addAlbumTile(final Album album) {
        final var imageView = new ImageView();
        imageView.setFitWidth(100);
        imageView.setFitHeight(100);
        imageView.setPreserveRatio(true);

        ThemePolicyHelper.clipAlbumArtwork(imageView);

        imageView.setImage(album.getImage());

        // Artist can be empty, if so it will just be an empty string
        final var artistName = (null == album.getArtist()) ? "" : album.getArtist().getName();
        final var year = (null == album.getReleaseDate()) ? "" : " - " + album.getReleaseDate().getYear();

        final var container = TextHelper.caption(
            album.getName() + "\n",
            artistName + year,
            14,
            12,
            140.0,
            TextAlignment.CENTER
        );

        container.setPadding(new Insets(5, 5, 5, 5));
        container.setOnMouseClicked(event -> this.onAlbumClick(event, album));
        container.setAlignment(Pos.TOP_CENTER);

        container.getChildren().add(0, imageView);

        albumList.getChildren().add(container);
    }

    @FXML private void closeSelectedAlbumContent(final ActionEvent actionEvent) {
        selectedAlbumContainer.setVisible(false);
    }

    // --- Listeners ---

    private void onAlbumClick(final MouseEvent event, final Album album) {
        selectedAlbumImageView.setImage(album.getImage());

        selectedAlbumLabel.setText(album.getName());

        selectedAlbumArtistLabel.setText(
            (null == album.getArtist())
            ? ""
            : album.getArtist().getName()
        );

        selectedAlbumTrackList.getChildren().clear();

        final var songs = albumRepository().getSongs(album.getDto()).stream()
            .map(Song::new)
            .toList();

        songs.forEach(this::addSongToSelectedAlbumTrackList);

        selectedAlbumContainer.setVisible(true);

        if (null != selectedAlbum) {
            selectedAlbum.setStyle("");
        }

        final var clickedItem = (VBox) event.getSource();
        clickedItem.setStyle("-fx-background-color: darkgray");
        selectedAlbum = clickedItem;
    }

    private void addSongToSelectedAlbumTrackList(final Song song) {
        final var songRow = new HBox();
        songRow.setSpacing(5);

        songRow.getChildren().add(new Text(song.getTrackNumber() + "."));
        songRow.getChildren().add(new Text(song.getTitle()));
        songRow.getChildren().add(new Text(DurationHelper.getTimeString(Duration.millis(song.getDuration()))));

        selectedAlbumTrackList.getChildren().add(songRow);
    }
}
