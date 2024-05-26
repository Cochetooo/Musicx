package fr.xahla.musicx.desktop.views.content.edit;

import fr.xahla.musicx.api.model.enums.ReleaseType;
import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.service.saveLocalSongs.WriteMetadataToAudioFile;
import javafx.concurrent.Task;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.*;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.stage.FileChooser;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.application.AbstractContext.albumRepository;

/**
 * View for album editing.
 * @author Cochetooo
 * @since 0.3.1
 */
public class AlbumEdit implements Initializable {

    @FXML private Button editButton;

    @FXML private Label artworkDimensionLabel;
    @FXML private TextField albumNameField;

    @FXML private TextField catalogNoField;
    @FXML private TextField discTotalField;
    @FXML private TextField trackTotalField;

    @FXML private ComboBox<ReleaseType> albumTypeComboBox;
    @FXML private DatePicker releaseDatePicker;

    @FXML private TextField artworkUrlField;
    @FXML private ImageView artworkView;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var album = audioPlayer().getEditedSong().getAlbum();

        this.albumNameField.setText(album.getName());
        this.albumNameField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.catalogNoField.setText(album.getCatalogNo());
        this.catalogNoField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.discTotalField.setText(String.valueOf(album.getDiscTotal()));
        this.discTotalField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.trackTotalField.setText(String.valueOf(album.getTrackTotal()));
        this.trackTotalField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.albumTypeComboBox.getItems().addAll(ReleaseType.values());
        this.albumTypeComboBox.setValue(album.getType());
        this.albumTypeComboBox.valueProperty().addListener((observable, oldValue, newValue) -> {
            if (oldValue != newValue) {
                change();
            }
        });

        this.releaseDatePicker.setValue(album.getReleaseDate());
        this.releaseDatePicker.valueProperty().addListener((observable, oldValue, newValue) -> {
            if (oldValue != newValue) {
                change();
            }
        });

        this.artworkUrlField.setText(album.getArtworkUrl());

        this.artworkDimensionLabel.setTextFill(ColorHelper.GRAY);

        this.artworkView.imageProperty().addListener((observable, oldValue, newValue)
            -> this.artworkDimensionLabel.setText(newValue.getWidth() + " x " + newValue.getHeight()));

        if (null != album.getArtworkUrl() && !album.getArtworkUrl().isEmpty()) {
            this.artworkView.setImage(new Image(album.getArtworkUrl()));
        }
    }

    @FXML public void editArtist() {

    }

    @FXML public void editLabel() {

    }

    @FXML public void change() {
        editButton.setDisable(false);
    }

    @FXML public void chooseArtwork() {
        final var fileChooser = new FileChooser();
        final var imageFilter = new FileChooser.ExtensionFilter("Images",
            "*.jpg", "*.jpeg", "*.png", "*.gif"
        );
        fileChooser.getExtensionFilters().add(imageFilter);

        final var selectedFile = fileChooser.showOpenDialog(null);

        if (null != selectedFile) {
            this.artworkUrlField.setText(selectedFile.getAbsolutePath());
            this.artworkView.setImage(new Image(artworkUrlField.getText()));
            this.change();
        }
    }

    @FXML public void edit() {
        final var album = audioPlayer().getEditedSong().getAlbum();

        album.setArtworkUrl(artworkUrlField.getText());
        album.setCatalogNo(catalogNoField.getText());
        album.setDiscTotal(StringHelper.str_parse_short_safe(discTotalField.getText()));
        album.setName(albumNameField.getText());
        album.setReleaseDate(releaseDatePicker.getValue());
        album.setTrackTotal(StringHelper.str_parse_short_safe(trackTotalField.getText()));
        album.setType(albumTypeComboBox.getValue());

        albumRepository().save(album.getDto());
        new WriteMetadataToAudioFile().execute(album.getDto());

        this.updateSongs(album);

        this.editButton.setDisable(true);
    }

    @FXML private void close() {
        scene().getRightNavContent().close();
    }

    private void updateSongs(final Album album) {
        final var updateTask = new Task<Void>() {
            @Override protected Void call() {
                final var songs = scene().getLocalLibraryScene().getLibrary().getLocalSongs().stream()
                    .filter(song -> null != song && song.getAlbum().getId() == album.getId())
                    .toList();

                songs.forEach(song -> song.setAlbum(album));
                return null;
            }
        };

        new Thread(updateTask).start();
    }
}
