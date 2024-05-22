package fr.xahla.musicx.desktop.views.content.localLibrary;

import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.geometry.Pos;
import javafx.scene.control.Label;
import javafx.scene.control.ListView;
import javafx.scene.image.ImageView;
import javafx.scene.layout.HBox;
import javafx.scene.layout.TilePane;
import javafx.scene.layout.VBox;
import javafx.scene.shape.Rectangle;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;
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

    @FXML private HBox selectedAlbumContainer;
    @FXML private ImageView selectedAlbumImageView;
    @FXML private Label selectedAlbumLabel;
    @FXML private Label selectedAlbumArtistLabel;
    @FXML private TilePane selectedAlbumTrackList;

    private ListProperty<Album> albums;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var albumsDto = albumRepository().findAll();

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
        imageView.setFitWidth(80);
        imageView.setFitHeight(80);
        imageView.setPreserveRatio(true);

        final var roundedBorderClip = new Rectangle(
            imageView.getFitWidth(),
            imageView.getFitHeight()
        );

        roundedBorderClip.setArcWidth(10);
        roundedBorderClip.setArcHeight(10);

        imageView.setClip(roundedBorderClip);

        final var image = (null == album.getImage()) ? Album.artworkPlaceholder : album.getImage();
        imageView.setImage(image);

        imageView.onMouseClickedProperty().addListener(
            (observable, oldValue, newValue) -> this.onAlbumClick(album)
        );

        final var albumText = new Text(album.getName());
        albumText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 14));
        albumText.setFill(ColorHelper.PRIMARY);

        // Artist can be empty, if so it will just be an empty string
        final var artistName = (null == album.getArtist()) ? "" : album.getArtist().getName();
        final var year = (null == album.getReleaseDate()) ? "" : " - " + album.getReleaseDate().getYear();

        final var artistText = new Text(artistName + year);
        artistText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, 12));
        artistText.setFill(ColorHelper.SECONDARY);

        final var vBox = new VBox();
        vBox.setAlignment(Pos.CENTER);

        vBox.getChildren().addAll(imageView, albumText, artistText);

        albumList.getChildren().add(vBox);
    }

    // --- Listeners ---

    private void onAlbumClick(final Album album) {
        selectedAlbumImageView.setImage(
            (null == album.getImage())
            ? Album.artworkPlaceholder
            : album.getImage()
        );

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

        logger().finest("Clicked");
        selectedAlbumContainer.setVisible(true);
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
